using System;
using System.Collections.Generic;
using System.Threading;
using VSC.TypeSystem.Implementation;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Compares parameter lists by comparing the types of all parameters.
	/// </summary>
	/// <remarks>
	/// 'ref int' and 'out int' are considered to be equal.
	/// 'object' and 'dynamic' are also equal.
	/// For generic methods, "Method{T}(T a)" and "Method{S}(S b)" are considered equal.
	/// However, "Method(T a)" and "Method(S b)" are not considered equal when the type parameters T and S belong to classes.
	/// </remarks>
	public sealed class ParameterListComparer : IEqualityComparer<IList<IParameter>>
	{
		public static readonly ParameterListComparer Instance = new ParameterListComparer();
		
		sealed class NormalizeTypeVisitor : TypeVisitor
		{
			public override IType VisitTypeParameter(ITypeParameter type)
			{
				if (type.OwnerType == SymbolKind.Method) {
					return DummyTypeParameter.GetMethodTypeParameter(type.Index);
				} else {
					return base.VisitTypeParameter(type);
				}
			}
			
			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				if (type.KnownTypeCode == KnownTypeCode.Object)
					return SpecialTypeSpec.Dynamic;
				return base.VisitTypeDefinition(type);
			}
		}
		
		static readonly NormalizeTypeVisitor normalizationVisitor = new NormalizeTypeVisitor();
		
		/// <summary>
		/// Replaces all occurrences of method type parameters in the given type
		/// by normalized type parameters. This allows comparing parameter types from different
		/// generic methods.
		/// </summary>
		[Obsolete("Use DummyTypeParameter.NormalizeMethodTypeParameters instead if you only need to normalize type parameters. Also, consider if you need to normalize object vs. dynamic as well.")]
		public IType NormalizeMethodTypeParameters(IType type)
		{
			return DummyTypeParameter.NormalizeMethodTypeParameters(type);
		}
		
		public bool Equals(IList<IParameter> x, IList<IParameter> y)
		{
			if (x == y)
				return true;
			if (x == null || y == null || x.Count != y.Count)
				return false;
			for (int i = 0; i < x.Count; i++) {
				var a = x[i];
				var b = y[i];
				if (a == null && b == null)
					continue;
				if (a == null || b == null)
					return false;
				
				// We want to consider the parameter lists "Method<T>(T a)" and "Method<S>(S b)" as equal.
				// However, the parameter types are not considered equal, as T is a different type parameter than S.
				// In order to compare the method signatures, we will normalize all method type parameters.
				IType aType = a.Type.AcceptVisitor(normalizationVisitor);
				IType bType = b.Type.AcceptVisitor(normalizationVisitor);
				
				if (!aType.Equals(bType))
					return false;
			}
			return true;
		}
		
		public int GetHashCode(IList<IParameter> obj)
		{
			int hashCode = obj.Count;
			unchecked {
				foreach (IParameter p in obj) {
					hashCode *= 27;
					IType type = p.Type.AcceptVisitor(normalizationVisitor);
					hashCode += type.GetHashCode();
				}
			}
			return hashCode;
		}
	}
}
