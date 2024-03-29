﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem.Implementation;


namespace VSC.TypeSystem.Resolver
{
    /// <summary>
	/// Implements V# 4.0 Type Inference (§7.5.2).
	/// </summary>
	public sealed class TypeInference
	{
		readonly ICompilation compilation;
		readonly VSharpConversions conversions;
        TypeInferenceAlgorithm algorithm = TypeInferenceAlgorithm.VSharp;
		
		// determines the maximum generic nesting level; necessary to avoid infinite recursion in 'Improved' mode.
		const int maxNestingLevel = 5;
		int nestingLevel;
		
		#region Constructor
		public TypeInference(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			this.compilation = compilation;
			this.conversions = VSharpConversions.Get(compilation);
		}
		
		internal TypeInference(ICompilation compilation, VSharpConversions conversions)
		{
			Debug.Assert(compilation != null);
			Debug.Assert(conversions != null);
			this.compilation = compilation;
			this.conversions = conversions;
		}
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets/Sets the type inference algorithm used.
		/// </summary>
		public TypeInferenceAlgorithm Algorithm {
			get { return algorithm; }
			set { algorithm = value; }
		}
		
		TypeInference CreateNestedInstance()
		{
			TypeInference c = new TypeInference(compilation, conversions);
			c.algorithm = algorithm;
			c.nestingLevel = nestingLevel + 1;
			return c;
		}
		#endregion
		
		TP[] typeParameters;
		IType[] parameterTypes;
        AST.Expression[] arguments;
		bool[,] dependencyMatrix;
		IList<IType> classTypeArguments;
		
		#region InferTypeArguments (main function)
		/// <summary>
		/// Performs type inference.
		/// </summary>
		/// <param name="typeParameters">The method type parameters that should be inferred.</param>
		/// <param name="arguments">The arguments passed to the method.</param>
		/// <param name="parameterTypes">The parameter types of the method.</param>
		/// <param name="success">Out: whether type inference was successful</param>
		/// <param name="classTypeArguments">
		/// Class type arguments. These are substituted for class type parameters in the formal parameter types
		/// when inferring a method group or lambda.
		/// </param>
		/// <returns>The inferred type arguments.</returns>
		public IType[] InferTypeArguments(IList<ITypeParameter> typeParameters, IList<AST.Expression> arguments, IList<IType> parameterTypes, out bool success, IList<IType> classTypeArguments = null)
		{
			if (typeParameters == null)
				throw new ArgumentNullException("typeParameters");
			if (arguments == null)
				throw new ArgumentNullException("arguments");
			if (parameterTypes == null)
				throw new ArgumentNullException("parameterTypes");
			try {
				this.typeParameters = new TP[typeParameters.Count];
				for (int i = 0; i < this.typeParameters.Length; i++) {
					if (i != typeParameters[i].Index)
						throw new ArgumentException("Type parameter has wrong index");
					if (typeParameters[i].OwnerType != SymbolKind.Method)
						throw new ArgumentException("Type parameter must be owned by a method");
					this.typeParameters[i] = new TP(typeParameters[i]);
				}
				this.parameterTypes = new IType[Math.Min(arguments.Count, parameterTypes.Count)];
				this.arguments = new Expression[this.parameterTypes.Length];
				for (int i = 0; i < this.parameterTypes.Length; i++) {
					if (arguments[i] == null || parameterTypes[i] == null)
						throw new ArgumentNullException();
					this.arguments[i] = arguments[i];
					this.parameterTypes[i] = parameterTypes[i];
				}
				this.classTypeArguments = classTypeArguments;
				
				PhaseOne();
				success = PhaseTwo();
			
				return this.typeParameters.Select(tp => tp.FixedTo ?? SpecialTypeSpec.UnknownType).ToArray();
			} finally {
				Reset();
			}
		}
		
		void Reset()
		{
			// clean up so that memory used by the operation can be garbage collected as soon as possible
			this.typeParameters = null;
			this.parameterTypes = null;
			this.arguments = null;
			this.dependencyMatrix = null;
			this.classTypeArguments = null;
		}
		
		/// <summary>
		/// Infers type arguments for the <paramref name="typeParameters"/> occurring in the <paramref name="targetType"/>
		/// so that the resulting type (after substition) satisfies the given bounds.
		/// </summary>
		public IType[] InferTypeArgumentsFromBounds(IList<ITypeParameter> typeParameters, IType targetType, IList<IType> lowerBounds, IList<IType> upperBounds, out bool success)
		{
			if (typeParameters == null)
				throw new ArgumentNullException("typeParameters");
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (lowerBounds == null)
				throw new ArgumentNullException("lowerBounds");
			if (upperBounds == null)
				throw new ArgumentNullException("upperBounds");
			this.typeParameters = new TP[typeParameters.Count];
			for (int i = 0; i < this.typeParameters.Length; i++) {
				if (i != typeParameters[i].Index)
					throw new ArgumentException("Type parameter has wrong index");
				this.typeParameters[i] = new TP(typeParameters[i]);
			}
			foreach (IType b in lowerBounds) {
				MakeLowerBoundInference(b, targetType);
			}
			foreach (IType b in upperBounds) {
				MakeUpperBoundInference(b, targetType);
			}
			IType[] result = new IType[this.typeParameters.Length];
			success = true;
			for (int i = 0; i < result.Length; i++) {
				success &= Fix(this.typeParameters[i]);
				result[i] = this.typeParameters[i].FixedTo ?? SpecialTypeSpec.UnknownType;
			}
			Reset();
			return result;
		}
		#endregion
		
		sealed class TP
		{
			public readonly HashSet<IType> LowerBounds = new HashSet<IType>();
			public readonly HashSet<IType> UpperBounds = new HashSet<IType>();
			public IType ExactBound;
			public bool MultipleDifferentExactBounds;
			public readonly ITypeParameter TypeParameter;
			public IType FixedTo;
			
			public bool IsFixed {
				get { return FixedTo != null; }
			}
			
			public bool HasBounds {
				get { return LowerBounds.Count > 0 || UpperBounds.Count > 0 || ExactBound != null; }
			}
			
			public TP(ITypeParameter typeParameter)
			{
				if (typeParameter == null)
					throw new ArgumentNullException("typeParameter");
				this.TypeParameter = typeParameter;
			}
			
			public void AddExactBound(IType type)
			{
				// Exact bounds need to stored separately, not just as Lower+Upper bounds,
				// due to TypeInferenceTests.GenericArgumentImplicitlyConvertibleToAndFromAnotherTypeList (see #281)
				if (ExactBound == null)
					ExactBound = type;
				else if (!ExactBound.Equals(type))
					MultipleDifferentExactBounds = true;
			}
			
			public override string ToString()
			{
				return TypeParameter.Name;
			}
		}
		
		sealed class OccursInVisitor : TypeVisitor
		{
			readonly TP[] tp;
			public readonly bool[] Occurs;
			
			public OccursInVisitor(TypeInference typeInference)
			{
				this.tp = typeInference.typeParameters;
				this.Occurs = new bool[tp.Length];
			}
			
			public override IType VisitTypeParameter(ITypeParameter type)
			{
				int index = type.Index;
				if (index < tp.Length && tp[index].TypeParameter == type)
					Occurs[index] = true;
				return base.VisitTypeParameter(type);
			}
		}
		
		#region Inference Phases
		void PhaseOne()
		{
			// V# 4.0 spec: §7.5.2.1 The first phase
		
			for (int i = 0; i < arguments.Length; i++) {
				Expression Ei = arguments[i];
				IType Ti = parameterTypes[i];
				
				LambdaExpression lrr = Ei as LambdaExpression;
				if (lrr != null) {
					MakeExplicitParameterTypeInference(lrr, Ti);
				}
				if (lrr != null || Ei is MethodGroupExpression) {
					// this is not in the spec???
					if (OutputTypeContainsUnfixed(Ei, Ti) && !InputTypesContainsUnfixed(Ei, Ti)) {
						MakeOutputTypeInference(Ei, Ti);
					}
				}
				
				if (IsValidType(Ei.Type)) {
					if (Ti is ByReferenceType) {
						MakeExactInference(Ei.Type, Ti);
					} else {
						MakeLowerBoundInference(Ei.Type, Ti);
					}
				}
			}
		}
		
		static bool IsValidType(IType type)
		{
			return type.Kind != TypeKind.Unknown && type.Kind != TypeKind.Null;
		}
		
		bool PhaseTwo()
		{
			// V# 4.0 spec: §7.5.2.2 The second phase
			// All unfixed type variables Xi which do not depend on any Xj are fixed.
			List<TP> typeParametersToFix = new List<TP>();
			foreach (TP Xi in typeParameters) {
				if (Xi.IsFixed == false) {
					if (!typeParameters.Any((TP Xj) => !Xj.IsFixed && DependsOn(Xi, Xj))) {
						typeParametersToFix.Add(Xi);
					}
				}
			}
			// If no such type variables exist, all unfixed type variables Xi are fixed for which all of the following hold:
			if (typeParametersToFix.Count == 0) {
				foreach (TP Xi in typeParameters) {
					// Xi has a non­empty set of bounds
					if (!Xi.IsFixed && Xi.HasBounds) {
						// There is at least one type variable Xj that depends on Xi
						if (typeParameters.Any((TP Xj) => DependsOn(Xj, Xi))) {
							typeParametersToFix.Add(Xi);
						}
					}
				}
			}
			// now fix 'em
			bool errorDuringFix = false;
			foreach (TP tp in typeParametersToFix) {
				if (!Fix(tp))
					errorDuringFix = true;
			}
			if (errorDuringFix)
				return false;
			bool unfixedTypeVariablesExist = typeParameters.Any((TP X) => X.IsFixed == false);
			if (typeParametersToFix.Count == 0 && unfixedTypeVariablesExist) {
				// If no such type variables exist and there are still unfixed type variables, type inference fails.
				return false;
			} else if (!unfixedTypeVariablesExist) {
				// Otherwise, if no further unfixed type variables exist, type inference succeeds.
				return true;
			} else {
				// Otherwise, for all arguments ei with corresponding parameter type Ti
				for (int i = 0; i < arguments.Length; i++) {
					Expression Ei = arguments[i];
					IType Ti = parameterTypes[i];
					// where the output types (§7.4.2.4) contain unfixed type variables Xj
					// but the input types (§7.4.2.3) do not
					if (OutputTypeContainsUnfixed(Ei, Ti) && !InputTypesContainsUnfixed(Ei, Ti)) {
						// an output type inference (§7.4.2.6) is made for ei with type Ti.
					
						MakeOutputTypeInference(Ei, Ti);
					}
				}
				// Then the second phase is repeated.
				return PhaseTwo();
			}
		}
		#endregion
		
		#region Input Types / Output Types (§7.5.2.3 + §7.5.2.4)
		static readonly IType[] emptyTypeArray = new IType[0];

        IType[] InputTypes(AST.Expression e, IType t)
		{
			// V# 4.0 spec: §7.5.2.3 Input types
			LambdaExpression lrr = e as LambdaExpression;
			if (lrr != null && lrr.IsImplicitlyTyped || e is MethodGroupExpression) {
				IMethod m = GetDelegateOrExpressionTreeSignature(t);
				if (m != null) {
					IType[] inputTypes = new IType[m.Parameters.Count];
					for (int i = 0; i < inputTypes.Length; i++) {
						inputTypes[i] = m.Parameters[i].Type;
					}
					return inputTypes;
				}
			}
			return emptyTypeArray;
		}

        IType[] OutputTypes(AST.Expression e, IType t)
		{
			// V# 4.0 spec: §7.5.2.4 Output types
			LambdaExpression lrr = e as LambdaExpression;
            if (lrr != null || e is MethodGroupExpression)
            {
				IMethod m = GetDelegateOrExpressionTreeSignature(t);
				if (m != null) {
					return new[] { m.ReturnType };
				}
			}
			return emptyTypeArray;
		}
		
		static IMethod GetDelegateOrExpressionTreeSignature(IType t)
		{
            ParameterizedTypeSpec pt = t as ParameterizedTypeSpec;
			if (pt != null && pt.TypeParameterCount == 1 && pt.Name == "Expression"
                && pt.Namespace == "Std.Linq.Expressions")
			{
				t = pt.GetTypeArgument(0);
			}
			return t.GetDelegateInvokeMethod();
		}

        bool InputTypesContainsUnfixed(AST.Expression argument, IType parameterType)
		{
			return AnyTypeContainsUnfixedParameter(InputTypes(argument, parameterType));
		}

        bool OutputTypeContainsUnfixed(AST.Expression argument, IType parameterType)
		{
			return AnyTypeContainsUnfixedParameter(OutputTypes(argument, parameterType));
		}
		
		bool AnyTypeContainsUnfixedParameter(IEnumerable<IType> types)
		{
			OccursInVisitor o = new OccursInVisitor(this);
			foreach (var type in types) {
				type.AcceptVisitor(o);
			}
			for (int i = 0; i < typeParameters.Length; i++) {
				if (!typeParameters[i].IsFixed && o.Occurs[i])
					return true;
			}
			return false;
		}
		#endregion
		
		#region DependsOn (§7.5.2.5)
		// V# 4.0 spec: §7.5.2.5 Dependance
		
		void CalculateDependencyMatrix()
		{
			int n = typeParameters.Length;
			dependencyMatrix = new bool[n, n];
			for (int k = 0; k < arguments.Length; k++) {
				OccursInVisitor input = new OccursInVisitor(this);
				OccursInVisitor output = new OccursInVisitor(this);
				foreach (var type in InputTypes(arguments[k], parameterTypes[k])) {
					type.AcceptVisitor(input);
				}
				foreach (var type in OutputTypes(arguments[k], parameterTypes[k])) {
					type.AcceptVisitor(output);
				}
				for (int i = 0; i < n; i++) {
					for (int j = 0; j < n; j++) {
						dependencyMatrix[i, j] |= input.Occurs[j] && output.Occurs[i];
					}
				}
			}
			// calculate transitive closure using Warshall's algorithm:
			for (int i = 0; i < n; i++) {
				for (int j = 0; j < n; j++) {
					if (dependencyMatrix[i, j]) {
						for (int k = 0; k < n; k++) {
							if (dependencyMatrix[j, k])
								dependencyMatrix[i, k] = true;
						}
					}
				}
			}
		}
		
		bool DependsOn(TP x, TP y)
		{
			if (dependencyMatrix == null)
				CalculateDependencyMatrix();
			// x depends on y
			return dependencyMatrix[x.TypeParameter.Index, y.TypeParameter.Index];
		}
		#endregion
		
		#region MakeOutputTypeInference (§7.5.2.6)
        void MakeOutputTypeInference(AST.Expression e, IType t)
		{
			
			// If E is an anonymous function with inferred return type  U (§7.5.2.12) and T is a delegate type or expression
			// tree type with return type Tb, then a lower-bound inference (§7.5.2.9) is made from U to Tb.
			LambdaExpression lrr = e as LambdaExpression;
			if (lrr != null) {
				IMethod m = GetDelegateOrExpressionTreeSignature(t);
				if (m != null) {
					IType inferredReturnType;
					if (lrr.IsImplicitlyTyped) {
						if (m.Parameters.Count != lrr.Parameters.Count)
							return; // cannot infer due to mismatched parameter lists
						TypeParameterSubstitution substitution = GetSubstitutionForFixedTPs();
						IType[] inferredParameterTypes = new IType[m.Parameters.Count];
						for (int i = 0; i < inferredParameterTypes.Length; i++) {
							IType parameterType = m.Parameters[i].Type;
							inferredParameterTypes[i] = parameterType.AcceptVisitor(substitution);
						}
						inferredReturnType = lrr.GetInferredReturnType(inferredParameterTypes);
					} else {
						inferredReturnType = lrr.GetInferredReturnType(null);
					}
					MakeLowerBoundInference(inferredReturnType, m.ReturnType);
					return;
				}
			}
			// Otherwise, if E is a method group and T is a delegate type or expression tree type
			// with parameter types T1…Tk and return type Tb, and overload resolution
			// of E with the types T1…Tk yields a single method with return type U, then a lower­-bound
			// inference is made from U to Tb.
            MethodGroupExpression mgrr = e as MethodGroupExpression;
			if (mgrr != null) {
				IMethod m = GetDelegateOrExpressionTreeSignature(t);
				if (m != null) {
					Expression[] args = new Expression[m.Parameters.Count];
					TypeParameterSubstitution substitution = GetSubstitutionForFixedTPs();
					for (int i = 0; i < args.Length; i++) {
						IParameter param = m.Parameters[i];
						IType parameterType = param.Type.AcceptVisitor(substitution);
						if ((param.IsRef || param.IsOut) && parameterType.Kind == TypeKind.ByReference) {
							parameterType = ((ByReferenceType)parameterType).ElementType;
							args[i] = new ByReferenceExpression(parameterType, param.IsOut);
						} else {
							args[i] = new Expression(parameterType);
						}
					}
					var or = mgrr.PerformOverloadResolution(compilation,
					                                        args,
					                                        allowExpandingParams: false, allowOptionalParameters: false);
					if (or.FoundApplicableCandidate && or.BestCandidateAmbiguousWith == null) {
						IType returnType = or.GetBestCandidateWithSubstitutedTypeArguments().ReturnType;
						MakeLowerBoundInference(returnType, m.ReturnType);
					}
				}
				return;
			}
			// Otherwise, if E is an expression with type U, then a lower-bound inference is made from U to T.
			if (IsValidType(e.Type)) {
				MakeLowerBoundInference(e.Type, t);
			}
		}
		
		TypeParameterSubstitution GetSubstitutionForFixedTPs()
		{
			IType[] fixedTypes = new IType[typeParameters.Length];
			for (int i = 0; i < fixedTypes.Length; i++) {
				fixedTypes[i] = typeParameters[i].FixedTo ?? SpecialTypeSpec.UnknownType;
			}
			return new TypeParameterSubstitution(classTypeArguments, fixedTypes);
		}
		#endregion
		
		#region MakeExplicitParameterTypeInference (§7.5.2.7)
		void MakeExplicitParameterTypeInference(LambdaExpression e, IType t)
		{
			// V# 4.0 spec: §7.5.2.7 Explicit parameter type inferences
			if (e.IsImplicitlyTyped || !e.HasParameterList)
				return;
		
			IMethod m = GetDelegateOrExpressionTreeSignature(t);
			if (m == null)
				return;
			for (int i = 0; i < e.Parameters.Count && i < m.Parameters.Count; i++) {
				MakeExactInference(e.Parameters[i].Type, m.Parameters[i].Type);
			}
		}
		#endregion
		
		#region MakeExactInference (§7.5.2.8)
		/// <summary>
		/// Make exact inference from U to V.
		/// V# 4.0 spec: §7.5.2.8 Exact inferences
		/// </summary>
		void MakeExactInference(IType U, IType V)
		{
			
			// If V is one of the unfixed Xi then U is added to the set of bounds for Xi.
			TP tp = GetTPForType(V);
			if (tp != null && tp.IsFixed == false) {
			
				tp.AddExactBound(U);
				return;
			}
			// Handle by reference types:
			ByReferenceType brU = U as ByReferenceType;
			ByReferenceType brV = V as ByReferenceType;
			if (brU != null && brV != null) {
				MakeExactInference(brU.ElementType, brV.ElementType);
				return;
			}
			// Handle array types:
			ArrayType arrU = U as ArrayType;
			ArrayType arrV = V as ArrayType;
			if (arrU != null && arrV != null && arrU.Dimensions == arrV.Dimensions) {
				MakeExactInference(arrU.ElementType, arrV.ElementType);
				return;
			}
			// Handle parameterized type:
            ParameterizedTypeSpec pU = U as ParameterizedTypeSpec;
            ParameterizedTypeSpec pV = V as ParameterizedTypeSpec;
			if (pU != null && pV != null
			    && object.Equals(pU.GetDefinition(), pV.GetDefinition())
			    && pU.TypeParameterCount == pV.TypeParameterCount)
			{
			
				for (int i = 0; i < pU.TypeParameterCount; i++) {
					MakeExactInference(pU.GetTypeArgument(i), pV.GetTypeArgument(i));
				}
			
			}
		}
		
		TP GetTPForType(IType v)
		{
			ITypeParameter p = v as ITypeParameter;
			if (p != null) {
				int index = p.Index;
				if (index < typeParameters.Length && typeParameters[index].TypeParameter == p)
					return typeParameters[index];
			}
			return null;
		}
		#endregion
		
		#region MakeLowerBoundInference (§7.5.2.9)
		/// <summary>
		/// Make lower bound inference from U to V.
		/// V# 4.0 spec: §7.5.2.9 Lower-bound inferences
		/// </summary>
		void MakeLowerBoundInference(IType U, IType V)
		{
			
			// If V is one of the unfixed Xi then U is added to the set of bounds for Xi.
			TP tp = GetTPForType(V);
			if (tp != null && tp.IsFixed == false) {
				
				tp.LowerBounds.Add(U);
				return;
			}
			// Handle nullable covariance:
			if (NullableType.IsNullable(U) && NullableType.IsNullable(V)) {
				MakeLowerBoundInference(NullableType.GetUnderlyingType(U), NullableType.GetUnderlyingType(V));
				return;
			}
			
			// Handle array types:
			ArrayType arrU = U as ArrayType;
			ArrayType arrV = V as ArrayType;
            ParameterizedTypeSpec pV = V as ParameterizedTypeSpec;
			if (arrU != null && arrV != null && arrU.Dimensions == arrV.Dimensions) {
				MakeLowerBoundInference(arrU.ElementType, arrV.ElementType);
				return;
			} else if (arrU != null && IsGenericInterfaceImplementedByArray(pV) && arrU.Dimensions == 1) {
				MakeLowerBoundInference(arrU.ElementType, pV.GetTypeArgument(0));
				return;
			}
			// Handle parameterized types:
			if (pV != null) {
                ParameterizedTypeSpec uniqueBaseType = null;
				foreach (IType baseU in U.GetAllBaseTypes()) {
                    ParameterizedTypeSpec pU = baseU as ParameterizedTypeSpec;
					if (pU != null && object.Equals(pU.GetDefinition(), pV.GetDefinition()) && pU.TypeParameterCount == pV.TypeParameterCount) {
						if (uniqueBaseType == null)
							uniqueBaseType = pU;
						else
							return; // cannot make an inference because it's not unique
					}
				}
			
				if (uniqueBaseType != null) {
					for (int i = 0; i < uniqueBaseType.TypeParameterCount; i++) {
						IType Ui = uniqueBaseType.GetTypeArgument(i);
						IType Vi = pV.GetTypeArgument(i);
						if (Ui.IsReferenceType == true) {
							// look for variance
							ITypeParameter Xi = pV.GetDefinition().TypeParameters[i];
					
									MakeExactInference(Ui, Vi);
								
							
						} else {
							// not known to be a reference type
							MakeExactInference(Ui, Vi);
						}
					}
				}
				
			}
		}
		
		static bool IsGenericInterfaceImplementedByArray(ParameterizedTypeSpec rt)
		{
			if (rt == null || rt.TypeParameterCount != 1)
				return false;
			switch (rt.GetDefinition().KnownTypeCode) {
				case KnownTypeCode.IEnumerableOfT:
				case KnownTypeCode.ICollectionOfT:
				case KnownTypeCode.IListOfT:
				case KnownTypeCode.IReadOnlyCollectionOfT:
				case KnownTypeCode.IReadOnlyListOfT:
					return true;
				default:
					return false;
			}
		}
		#endregion
		
		#region MakeUpperBoundInference (§7.5.2.10)
		/// <summary>
		/// Make upper bound inference from U to V.
		/// V# 4.0 spec: §7.5.2.10 Upper-bound inferences
		/// </summary>
		void MakeUpperBoundInference(IType U, IType V)
		{
		
			
			// If V is one of the unfixed Xi then U is added to the set of bounds for Xi.
			TP tp = GetTPForType(V);
			if (tp != null && tp.IsFixed == false) {
				tp.UpperBounds.Add(U);
				return;
			}
			
			// Handle array types:
			ArrayType arrU = U as ArrayType;
			ArrayType arrV = V as ArrayType;
            ParameterizedTypeSpec pU = U as ParameterizedTypeSpec;
			if (arrV != null && arrU != null && arrU.Dimensions == arrV.Dimensions) {
				MakeUpperBoundInference(arrU.ElementType, arrV.ElementType);
				return;
			} else if (arrV != null && IsGenericInterfaceImplementedByArray(pU) && arrV.Dimensions == 1) {
				MakeUpperBoundInference(pU.GetTypeArgument(0), arrV.ElementType);
				return;
			}
			// Handle parameterized types:
			if (pU != null) {
                ParameterizedTypeSpec uniqueBaseType = null;
				foreach (IType baseV in V.GetAllBaseTypes()) {
                    ParameterizedTypeSpec pV = baseV as ParameterizedTypeSpec;
					if (pV != null && object.Equals(pU.GetDefinition(), pV.GetDefinition()) && pU.TypeParameterCount == pV.TypeParameterCount) {
						if (uniqueBaseType == null)
							uniqueBaseType = pV;
						else
							return; // cannot make an inference because it's not unique
					}
				}
			
				if (uniqueBaseType != null) {
					for (int i = 0; i < uniqueBaseType.TypeParameterCount; i++) {
						IType Ui = pU.GetTypeArgument(i);
						IType Vi = uniqueBaseType.GetTypeArgument(i);
						if (Ui.IsReferenceType == true) {
							// look for variance
							ITypeParameter Xi = pU.GetDefinition().TypeParameters[i];
							MakeExactInference(Ui, Vi);
							
						} else {
							// not known to be a reference type
							MakeExactInference(Ui, Vi);
						}
					}
				}
			
			}
		}
		#endregion
		
		#region Fixing (§7.5.2.11)
		bool Fix(TP tp)
		{
			
			Debug.Assert(!tp.IsFixed);
			if (tp.ExactBound != null) {
				// the exact bound will always be the result
				tp.FixedTo = tp.ExactBound;
				// check validity
				if (tp.MultipleDifferentExactBounds)
					return false;
				return tp.LowerBounds.All(b => conversions.ImplicitConversion(b, tp.FixedTo).IsValid)
					&& tp.UpperBounds.All(b => conversions.ImplicitConversion(tp.FixedTo, b).IsValid);
			}
			var types = CreateNestedInstance().FindTypesInBounds(tp.LowerBounds.ToArray(), tp.UpperBounds.ToArray());
			if (algorithm == TypeInferenceAlgorithm.ImprovedReturnAllResults) {
				tp.FixedTo = IntersectionType.Create(types);
				
				return types.Count >= 1;
			} else {
				tp.FixedTo = GetFirstTypePreferNonInterfaces(types);
				return types.Count == 1;
			}
		}
		#endregion
		
		#region Finding the best common type of a set of expresssions
		/// <summary>
		/// Gets the best common type (V# 4.0 spec: §7.5.2.14) of a set of expressions.
		/// </summary>
        public IType GetBestCommonType(IList<AST.Expression> expressions, out bool success)
		{
			if (expressions == null)
				throw new ArgumentNullException("expressions");
			if (expressions.Count == 1) {
				success = (expressions[0].Type.Kind != TypeKind.Unknown);
				return expressions[0].Type;
			}
			try {
				ITypeParameter tp = DummyTypeParameter.GetMethodTypeParameter(0);
				this.typeParameters = new TP[1] { new TP(tp) };
				foreach (Expression r in expressions) {
					MakeOutputTypeInference(r, tp);
				}
				success = Fix(typeParameters[0]);
				return typeParameters[0].FixedTo ?? SpecialTypeSpec.UnknownType;
			} finally {
				Reset();
			}
		}
		#endregion
		
		#region FindTypeInBounds
		/// <summary>
		/// Finds a type that satisfies the given lower and upper bounds.
		/// </summary>
		public IType FindTypeInBounds(IList<IType> lowerBounds, IList<IType> upperBounds)
		{
			if (lowerBounds == null)
				throw new ArgumentNullException("lowerBounds");
			if (upperBounds == null)
				throw new ArgumentNullException("upperBounds");
			
			IList<IType> result = FindTypesInBounds(lowerBounds, upperBounds);
			
			if (algorithm == TypeInferenceAlgorithm.ImprovedReturnAllResults) {
				return IntersectionType.Create(result);
			} else {
				// return any of the candidates (prefer non-interfaces)
				return GetFirstTypePreferNonInterfaces(result);
			}
		}
		
		static IType GetFirstTypePreferNonInterfaces(IList<IType> result)
		{
			return result.FirstOrDefault(c => c.Kind != TypeKind.Interface)
				?? result.FirstOrDefault() ?? SpecialTypeSpec.UnknownType;
		}
		
		IList<IType> FindTypesInBounds(IList<IType> lowerBounds, IList<IType> upperBounds)
		{
			// If there's only a single type; return that single type.
			// If both inputs are empty, return the empty list.
			if (lowerBounds.Count == 0 && upperBounds.Count <= 1)
				return upperBounds;
			if (upperBounds.Count == 0 && lowerBounds.Count <= 1)
				return lowerBounds;
			if (nestingLevel > maxNestingLevel)
				return EmptyList<IType>.Instance;
			
			// Finds a type X so that "LB <: X <: UB"
			
			// First try the Fixing algorithm from the V# spec (§7.5.2.11)
			List<IType> candidateTypes = lowerBounds.Union(upperBounds)
				.Where(c => lowerBounds.All(b => conversions.ImplicitConversion(b, c).IsValid))
				.Where(c => upperBounds.All(b => conversions.ImplicitConversion(c, b).IsValid))
				.ToList(); // evaluate the query only once

			
			
			// According to the V# specification, we need to pick the most specific
			// of the candidate types. (the type which has conversions to all others)
			// However, csc actually seems to choose the least specific.
			candidateTypes = candidateTypes.Where(
				c => candidateTypes.All(o => conversions.ImplicitConversion(o, c).IsValid)
			).ToList();

			// If the specified algorithm produces a single candidate, we return
			// that candidate.
			// We also return the whole candidate list if we're not using the improved
			// algorithm.
			if (candidateTypes.Count == 1 || !(algorithm == TypeInferenceAlgorithm.Improved || algorithm == TypeInferenceAlgorithm.ImprovedReturnAllResults))
			{
				return candidateTypes;
			}
			candidateTypes.Clear();
			
			// Now try the improved algorithm
		
			List<ITypeDefinition> candidateTypeDefinitions;
			if (lowerBounds.Count > 0) {
				// Find candidates by using the lower bounds:
				var hashSet = new HashSet<ITypeDefinition>(lowerBounds[0].GetAllBaseTypeDefinitions());
				for (int i = 1; i < lowerBounds.Count; i++) {
					hashSet.IntersectWith(lowerBounds[i].GetAllBaseTypeDefinitions());
				}
				candidateTypeDefinitions = hashSet.ToList();
			} else {
				// Find candidates by looking at all classes in the project:
				candidateTypeDefinitions = compilation.GetAllTypeDefinitions().ToList();
			}
			
			// Now filter out candidates that violate the upper bounds:
			foreach (IType ub in upperBounds) {
				ITypeDefinition ubDef = ub.GetDefinition();
				if (ubDef != null) {
					candidateTypeDefinitions.RemoveAll(c => !c.IsDerivedFrom(ubDef));
				}
			}
			
			foreach (ITypeDefinition candidateDef in candidateTypeDefinitions) {
				// determine the type parameters for the candidate:
				IType candidate;
				if (candidateDef.TypeParameterCount == 0) {
					candidate = candidateDef;
				} else {
			
					bool success;
					IType[] result = InferTypeArgumentsFromBounds(
						candidateDef.TypeParameters,
                        new ParameterizedTypeSpec(candidateDef, candidateDef.TypeParameters),
						lowerBounds, upperBounds,
						out success);
					if (success) {
                        candidate = new ParameterizedTypeSpec(candidateDef, result);
					} else {
				
						continue;
					}
				}
		
				
				if (upperBounds.Count == 0) {
					// if there were only lower bounds, we aim for the most specific candidate:
					
					// if this candidate isn't made redundant by an existing, more specific candidate:
					if (!candidateTypes.Any(c => c.GetDefinition().IsDerivedFrom(candidateDef))) {
						// remove all existing candidates made redundant by this candidate:
						candidateTypes.RemoveAll(c => candidateDef.IsDerivedFrom(c.GetDefinition()));
						// add new candidate
						candidateTypes.Add(candidate);
					}
				} else {
					// if there were upper bounds, we aim for the least specific candidate:
					
					// if this candidate isn't made redundant by an existing, less specific candidate:
					if (!candidateTypes.Any(c => candidateDef.IsDerivedFrom(c.GetDefinition()))) {
						// remove all existing candidates made redundant by this candidate:
						candidateTypes.RemoveAll(c => c.GetDefinition().IsDerivedFrom(candidateDef));
						// add new candidate
						candidateTypes.Add(candidate);
					}
				}
			}
		
			return candidateTypes;
		}
		#endregion
	}
}
