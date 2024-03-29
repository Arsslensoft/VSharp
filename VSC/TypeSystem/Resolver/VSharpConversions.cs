using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem;
using Expression = VSC.AST.Expression;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Contains logic that determines whether an implicit conversion exists between two types.
	/// </summary>
	/// <remarks>
	/// This class is thread-safe.
	/// </remarks>
	public sealed class VSharpConversions
	{
		readonly ConcurrentDictionary<TypePair, Conversion> implicitConversionCache = new ConcurrentDictionary<TypePair, Conversion>();
		readonly ICompilation compilation;
		readonly IType objectType;

        public VSharpConversions(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			this.compilation = compilation;
			this.objectType = compilation.FindType(KnownTypeCode.Object);
			this.dynamicErasure = new DynamicErasure(this);
		}
		
		/// <summary>
		/// Gets the Conversions instance for the specified <see cref="ICompilation"/>.
		/// This will make use of the context's cache manager to reuse the Conversions instance.
		/// </summary>
        public static VSharpConversions Get(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			CacheManager cache = compilation.CacheManager;
            VSharpConversions operators = (VSharpConversions)cache.GetShared(typeof(VSharpConversions));
			if (operators == null) {
                operators = (VSharpConversions)cache.GetOrAddShared(typeof(VSharpConversions), new VSharpConversions(compilation));
			}
			return operators;
		}
		
		#region TypePair (for caching)
		struct TypePair : IEquatable<TypePair>
		{
			public readonly IType FromType;
			public readonly IType ToType;
			
			public TypePair(IType fromType, IType toType)
			{
				Debug.Assert(fromType != null && toType != null);
				this.FromType = fromType;
				this.ToType = toType;
			}
			
			public override bool Equals(object obj)
			{
				return (obj is TypePair) && Equals((TypePair)obj);
			}
			
			public bool Equals(TypePair other)
			{
				return object.Equals(this.FromType, other.FromType) && object.Equals(this.ToType, other.ToType);
			}
			
			public override int GetHashCode()
			{
				unchecked {
					return 1000000007 * FromType.GetHashCode() + 1000000009 * ToType.GetHashCode();
				}
			}
		}
		#endregion
		
		#region ImplicitConversion
        private Conversion ImplicitConversion(AST.Expression resolveResult, IType toType, bool allowUserDefined)
		{
			Conversion c;
			if (resolveResult.IsCompileTimeConstant) {
				c = ImplicitEnumerationConversion(resolveResult, toType);
				if (c.IsValid) return c;
				if (ImplicitConstantExpressionConversion(resolveResult, toType))
					return Conversion.ImplicitConstantExpressionConversion;
				c = StandardImplicitConversion(resolveResult.Type, toType);
				if (c != Conversion.None) return c;
				if (allowUserDefined) {
					c = UserDefinedImplicitConversion(resolveResult, resolveResult.Type, toType);
					if (c != Conversion.None) return c;
				}
			} else {
				c = ImplicitConversion(resolveResult.Type, toType, allowUserDefined);
				if (c != Conversion.None) return c;
			}
			if (resolveResult.Type.Kind == TypeKind.Dynamic)
				return Conversion.ImplicitDynamicConversion;
			c = AnonymousFunctionConversion(resolveResult, toType);
		
			return c;
		}
		
		private Conversion ImplicitConversion(IType fromType, IType toType, bool allowUserDefined)
		{
			// V# 4.0 spec: §6.1
			var c = StandardImplicitConversion(fromType, toType);
			if (c == Conversion.None && allowUserDefined) {
				c = UserDefinedImplicitConversion(null, fromType, toType);
			}
			return c;
		}

		public Conversion ImplicitConversion(AST.Expression resolveResult, IType toType)
		{
			if (resolveResult == null)
				throw new ArgumentNullException("resolveResult");
			return ImplicitConversion(resolveResult, toType, allowUserDefined: true);
		}

		public Conversion ImplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			TypePair pair = new TypePair(fromType, toType);
			Conversion c;
			if (implicitConversionCache.TryGetValue(pair, out c))
				return c;

			c = ImplicitConversion(fromType, toType, allowUserDefined: true);

			implicitConversionCache[pair] = c;
			return c;
		}
		
		public Conversion StandardImplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			// V# 4.0 spec: §6.3.1
			if (IdentityConversion(fromType, toType))
				return Conversion.IdentityConversion;
			if (ImplicitNumericConversion(fromType, toType))
				return Conversion.ImplicitNumericConversion;
			Conversion c = ImplicitNullableConversion(fromType, toType);
			if (c != Conversion.None)
				return c;
			if (NullLiteralConversion(fromType, toType))
				return Conversion.NullLiteralConversion;
			if (ImplicitReferenceConversion(fromType, toType, 0))
				return Conversion.ImplicitReferenceConversion;
			if (IsBoxingConversion(fromType, toType))
				return Conversion.BoxingConversion;
			if (ImplicitTypeParameterConversion(fromType, toType)) {
				// Implicit type parameter conversions that aren't also
				// reference conversions are considered to be boxing conversions
				return Conversion.BoxingConversion;
			}
			if (ImplicitPointerConversion(fromType, toType))
				return Conversion.ImplicitPointerConversion;
			return Conversion.None;
		}
		
		/// <summary>
		/// Gets whether the type 'fromType' is convertible to 'toType'
		/// using one of the conversions allowed when satisying constraints (§4.4.4)
		/// </summary>
		public bool IsConstraintConvertible(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			if (IdentityConversion(fromType, toType))
				return true;
			if (ImplicitReferenceConversion(fromType, toType, 0))
				return true;
			if (NullableType.IsNullable(fromType)) {
				// An 'object' constraint still allows nullable value types
				// (object constraints don't exist in V#, but are inserted by DefaultResolvedTypeParameter.DirectBaseTypes)
				if (toType.IsKnownType(KnownTypeCode.Object))
					return true;
			} else {
				if (IsBoxingConversion(fromType, toType))
					return true;
			}
			if (ImplicitTypeParameterConversion(fromType, toType))
				return true;
			return false;
		}
		#endregion
		
		#region ExplicitConversion
		public Conversion ExplicitConversion(Expression expression, IType toType)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			if (expression.Type.Kind == TypeKind.Dynamic)
				return Conversion.ExplicitDynamicConversion;
			Conversion c = ImplicitConversion(expression, toType, allowUserDefined: false);
			if (c != Conversion.None)
				return c;
			c = ExplicitConversionImpl(expression.Type, toType);
			if (c != Conversion.None)
				return c;
			return UserDefinedExplicitConversion(expression, expression.Type, toType);
		}
		
		public Conversion ExplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			
			Conversion c = ImplicitConversion(fromType, toType, allowUserDefined: false);
			if (c != Conversion.None)
				return c;
			c = ExplicitConversionImpl(fromType, toType);
			if (c != Conversion.None)
				return c;
			return UserDefinedExplicitConversion(null, fromType, toType);
		}
		
		Conversion ExplicitConversionImpl(IType fromType, IType toType)
		{
			// This method is called after we already checked for implicit conversions,
			// so any remaining conversions must be explicit.
			if (AnyNumericConversion(fromType, toType))
				return Conversion.ExplicitNumericConversion;
			if (ExplicitEnumerationConversion(fromType, toType))
				return Conversion.EnumerationConversion(false, false);
			Conversion c = ExplicitNullableConversion(fromType, toType);
			if (c != Conversion.None)
				return c;
			if (ExplicitReferenceConversion(fromType, toType))
				return Conversion.ExplicitReferenceConversion;
			if (UnboxingConversion(fromType, toType))
				return Conversion.UnboxingConversion;
			c = ExplicitTypeParameterConversion(fromType, toType);
			if (c != Conversion.None)
				return c;
			if (ExplicitPointerConversion(fromType, toType))
				return Conversion.ExplicitPointerConversion;
			return Conversion.None;
		}
		#endregion
		
		#region Identity Conversion
		/// <summary>
		/// Gets whether there is an identity conversion from <paramref name="fromType"/> to <paramref name="toType"/>
		/// </summary>
		public bool IdentityConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.1
			return fromType.AcceptVisitor(dynamicErasure).Equals(toType.AcceptVisitor(dynamicErasure));
		}
		
		readonly DynamicErasure dynamicErasure;
		
		sealed class DynamicErasure : TypeVisitor
		{
			readonly IType objectType;

            public DynamicErasure(VSharpConversions conversions)
			{
				this.objectType = conversions.objectType;
			}
			
			public override IType VisitOtherType(IType type)
			{
				if (type.Kind == TypeKind.Dynamic)
					return objectType;
				else
					return base.VisitOtherType(type);
			}
		}
		#endregion
		
		#region Numeric Conversions
		static readonly bool[,] implicitNumericConversionLookup = {
			//       to:   short  ushort  int   uint   long   ulong
			// from:
			/* char   */ { false, true , true , true , true , true  },
			/* sbyte  */ { true , false, true , false, true , false },
			/* byte   */ { true , true , true , true , true , true  },
			/* short  */ { false, false, true , false, true , false },
			/* ushort */ { false, false, true , true , true , true  },
			/* int    */ { false, false, false, false, true , false },
			/* uint   */ { false, false, false, false, true , true  },
		};
		
		bool ImplicitNumericConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.2
			
			TypeCode from = ReflectionHelper.GetTypeCode(fromType);
			TypeCode to = ReflectionHelper.GetTypeCode(toType);
			if (to >= TypeCode.Single && to <= TypeCode.Decimal) {
				// Conversions to float/double/decimal exist from all integral types,
				// and there's a conversion from float to double.
				return from >= TypeCode.Char && from <= TypeCode.UInt64
					|| from == TypeCode.Single && to == TypeCode.Double;
			} else {
				// Conversions to integral types: look at the table
				return from >= TypeCode.Char && from <= TypeCode.UInt32
					&& to >= TypeCode.Int16 && to <= TypeCode.UInt64
					&& implicitNumericConversionLookup[from - TypeCode.Char, to - TypeCode.Int16];
			}
		}
		
		bool IsNumericType(IType type)
		{
			TypeCode c = ReflectionHelper.GetTypeCode(type);
			return c >= TypeCode.Char && c <= TypeCode.Decimal;
		}
		
		bool AnyNumericConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.2 + §6.2.1
			return IsNumericType(fromType) && IsNumericType(toType);
		}
		#endregion
		
		#region Enumeration Conversions
        Conversion ImplicitEnumerationConversion(AST.Expression rr, IType toType)
		{
			// V# 4.0 spec: §6.1.3
			Debug.Assert(rr.IsCompileTimeConstant);
			TypeCode constantType = ReflectionHelper.GetTypeCode(rr.Type);
			if (constantType >= TypeCode.SByte && constantType <= TypeCode.Decimal && Convert.ToDouble(rr.ConstantValue) == 0) {
				if (NullableType.GetUnderlyingType(toType).Kind == TypeKind.Enum) {
					return Conversion.EnumerationConversion(true, NullableType.IsNullable(toType));
				}
			}
			return Conversion.None;
		}
		
		bool ExplicitEnumerationConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.2.2
			if (fromType.Kind == TypeKind.Enum) {
				return toType.Kind == TypeKind.Enum || IsNumericType(toType);
			} else if (IsNumericType(fromType)) {
				return toType.Kind == TypeKind.Enum;
			}
			return false;
		}
		#endregion
		
		#region Nullable Conversions
		Conversion ImplicitNullableConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.4
			if (NullableType.IsNullable(toType)) {
				IType t = NullableType.GetUnderlyingType(toType);
				IType s = NullableType.GetUnderlyingType(fromType); // might or might not be nullable
				if (IdentityConversion(s, t))
					return Conversion.ImplicitNullableConversion;
				if (ImplicitNumericConversion(s, t))
					return Conversion.ImplicitLiftedNumericConversion;
			}
			return Conversion.None;
		}
		
		Conversion ExplicitNullableConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.4
			if (NullableType.IsNullable(toType) || NullableType.IsNullable(fromType)) {
				IType t = NullableType.GetUnderlyingType(toType);
				IType s = NullableType.GetUnderlyingType(fromType);
				if (IdentityConversion(s, t))
					return Conversion.ExplicitNullableConversion;
				if (AnyNumericConversion(s, t))
					return Conversion.ExplicitLiftedNumericConversion;
				if (ExplicitEnumerationConversion(s, t))
					return Conversion.EnumerationConversion(false, true);
			}
			return Conversion.None;
		}
		#endregion
		
		#region Null Literal Conversion
		bool NullLiteralConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.5
			if (fromType.Kind == TypeKind.Null) {
				return NullableType.IsNullable(toType) || toType.IsReferenceType == true;
			} else {
				return false;
			}
		}
		#endregion
		
		#region Implicit Reference Conversion
		public bool IsImplicitReferenceConversion(IType fromType, IType toType)
		{
			return ImplicitReferenceConversion(fromType, toType, 0);
		}
		
		bool ImplicitReferenceConversion(IType fromType, IType toType, int subtypeCheckNestingDepth)
		{
			// V# 4.0 spec: §6.1.6
			
			// reference conversions are possible:
			// - if both types are known to be reference types
			// - if both types are type parameters and fromType has a class constraint
			//     (ImplicitTypeParameterConversionWithClassConstraintOnlyOnT)
			if (!(fromType.IsReferenceType == true && toType.IsReferenceType != false))
				return false;
			
			ArrayType fromArray = fromType as ArrayType;
			if (fromArray != null) {
				ArrayType toArray = toType as ArrayType;
				if (toArray != null) {
					// array covariance (the broken kind)
					return fromArray.Dimensions == toArray.Dimensions
						&& ImplicitReferenceConversion(fromArray.ElementType, toArray.ElementType, subtypeCheckNestingDepth);
				}
				// conversion from single-dimensional array S[] to IList<T>:
				IType toTypeArgument = UnpackGenericArrayInterface(toType);
				if (fromArray.Dimensions == 1 && toTypeArgument != null) {
					// array covariance plays a part here as well (string[] is IList<object>)
					return IdentityConversion(fromArray.ElementType, toTypeArgument)
						|| ImplicitReferenceConversion(fromArray.ElementType, toTypeArgument, subtypeCheckNestingDepth);
				}
				// conversion from any array to System.Array and the interfaces it implements:
				IType systemArray = compilation.FindType(KnownTypeCode.Array);
				return ImplicitReferenceConversion(systemArray, toType, subtypeCheckNestingDepth);
			}
			
			// now comes the hard part: traverse the inheritance chain and figure out generics+variance
			return IsSubtypeOf(fromType, toType, subtypeCheckNestingDepth);
		}
		
		/// <summary>
		/// For IList{T}, ICollection{T}, IEnumerable{T} and IReadOnlyList{T}, returns T.
		/// Otherwise, returns null.
		/// </summary>
		IType UnpackGenericArrayInterface(IType interfaceType)
		{
            ParameterizedTypeSpec pt = interfaceType as ParameterizedTypeSpec;
			if (pt != null) {
				KnownTypeCode tc = pt.GetDefinition().KnownTypeCode;
				if (tc == KnownTypeCode.IListOfT || tc == KnownTypeCode.ICollectionOfT || tc == KnownTypeCode.IEnumerableOfT || tc == KnownTypeCode.IReadOnlyListOfT) {
					return pt.GetTypeArgument(0);
				}
			}
			return null;
		}
		
		// Determines whether s is a subtype of t.
		// Helper method used for ImplicitReferenceConversion, BoxingConversion and ImplicitTypeParameterConversion
		
		bool IsSubtypeOf(IType s, IType t, int subtypeCheckNestingDepth)
		{
			// conversion to dynamic + object are always possible
			if (t.Kind == TypeKind.Dynamic || t.Equals(objectType))
				return true;
			if (subtypeCheckNestingDepth > 10) {
				// Subtyping in V# is undecidable
				// (see "On Decidability of Nominal Subtyping with Variance" by Andrew J. Kennedy and Benjamin C. Pierce),
				// so we'll prevent infinite recursions by putting a limit on the nesting depth of variance conversions.
				
				// No real V# code should use generics nested more than 10 levels deep, and even if they do, most of
				// those nestings should not involve variance.
				return false;
			}
			// let GetAllBaseTypes do the work for us
			foreach (IType baseType in s.GetAllBaseTypes()) {
				if (IdentityOrVarianceConversion(baseType, t, subtypeCheckNestingDepth + 1))
					return true;
			}
			return false;
		}
		
		bool IdentityOrVarianceConversion(IType s, IType t, int subtypeCheckNestingDepth)
		{
			ITypeDefinition def = s.GetDefinition();
			if (def != null) {
				if (!def.Equals(t.GetDefinition()))
					return false;
                ParameterizedTypeSpec ps = s as ParameterizedTypeSpec;
                ParameterizedTypeSpec pt = t as ParameterizedTypeSpec;
				if (ps != null && pt != null) {
					// V# 4.0 spec: §13.1.3.2 Variance Conversion
					for (int i = 0; i < def.TypeParameters.Count; i++) {
						IType si = ps.GetTypeArgument(i);
						IType ti = pt.GetTypeArgument(i);
						if (IdentityConversion(si, ti))
							continue;
						ITypeParameter xi = def.TypeParameters[i];
	
								return false;
						
					}
				} else if (ps != null || pt != null) {
					return false; // only of of them is parameterized, or counts don't match? -> not valid conversion
				}
				return true;
			} else {
				// not type definitions? we still need to check for equal types (e.g. s and t might be type parameters)
				return s.Equals(t);
			}
		}
		#endregion
		
		#region Explicit Reference Conversion
		bool ExplicitReferenceConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.2.4
			
			// test that the types are reference types:
			if (toType.IsReferenceType != true)
				return false;
			if (fromType.IsReferenceType != true) {
				// special case:
				// converting from F to T is a reference conversion where T : class, F
				// (because F actually must be a reference type as well, even though V# doesn't treat it as one)
				if (fromType.Kind == TypeKind.TypeParameter)
					return IsSubtypeOf(toType, fromType, 0);
				return false;
			}
			
			if (toType.Kind == TypeKind.Array) {
				ArrayType toArray = (ArrayType)toType;
				if (fromType.Kind == TypeKind.Array) {
					// Array covariance
					ArrayType fromArray = (ArrayType)fromType;
					if (fromArray.Dimensions != toArray.Dimensions)
						return false;
					return ExplicitReferenceConversion(fromArray.ElementType, toArray.ElementType);
				}
				IType fromTypeArgument = UnpackGenericArrayInterface(fromType);
				if (fromTypeArgument != null && toArray.Dimensions == 1) {
					return ExplicitReferenceConversion(fromTypeArgument, toArray.ElementType)
						|| IdentityConversion(fromTypeArgument, toArray.ElementType);
				}
				// Otherwise treat the array like a sealed class - require implicit conversion in the opposite direction
				return IsImplicitReferenceConversion(toType, fromType);
			} else if (fromType.Kind == TypeKind.Array) {
				ArrayType fromArray = (ArrayType)fromType;
				IType toTypeArgument = UnpackGenericArrayInterface(toType);
				if (toTypeArgument != null && fromArray.Dimensions == 1) {
					return ExplicitReferenceConversion(fromArray.ElementType, toTypeArgument);
				}
				// Otherwise treat the array like a sealed class
				return IsImplicitReferenceConversion(fromType, toType);
			} else if (fromType.Kind == TypeKind.Delegate && toType.Kind == TypeKind.Delegate) {
				ITypeDefinition def = fromType.GetDefinition();
				if (def == null || !def.Equals(toType.GetDefinition()))
					return false;
                ParameterizedTypeSpec ps = fromType as ParameterizedTypeSpec;
                ParameterizedTypeSpec pt = toType as ParameterizedTypeSpec;
				if (ps == null || pt == null) {
					// non-generic delegate - return true for the identity conversion
					return ps == null && pt == null;
				}
				for (int i = 0; i < def.TypeParameters.Count; i++) {
					IType si = ps.GetTypeArgument(i);
					IType ti = pt.GetTypeArgument(i);
					if (IdentityConversion(si, ti))
						continue;
					ITypeParameter xi = def.TypeParameters[i];
				
							return false;
					
				}
				return true;
			} else if (IsSealedReferenceType(fromType)) {
				// If the source type is sealed, explicit conversions can't do anything more than implicit ones
				return IsImplicitReferenceConversion(fromType, toType);
			} else if (IsSealedReferenceType(toType)) {
				// The the target type is sealed, there must be an implicit conversion in the opposite direction
				return IsImplicitReferenceConversion(toType, fromType);
			} else {
				if (fromType.Kind == TypeKind.Interface || toType.Kind == TypeKind.Interface)
					return true;
				else
					return IsImplicitReferenceConversion(toType, fromType)
						|| IsImplicitReferenceConversion(fromType, toType);
			}
		}
		
		bool IsSealedReferenceType(IType type)
		{
			TypeKind kind = type.Kind;
			return kind == TypeKind.Class && type.GetDefinition().IsSealed
				|| kind == TypeKind.Delegate || kind == TypeKind.Anonymous;
		}
		#endregion
		
		#region Boxing Conversions
		public bool IsBoxingConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.1.7
			fromType = NullableType.GetUnderlyingType(fromType);
			if (fromType.IsReferenceType == false && toType.IsReferenceType == true)
				return IsSubtypeOf(fromType, toType, 0);
			else
				return false;
		}
		
		bool UnboxingConversion(IType fromType, IType toType)
		{
			// V# 4.0 spec: §6.2.5
			toType = NullableType.GetUnderlyingType(toType);
			if (fromType.IsReferenceType == true && toType.IsReferenceType == false)
				return IsSubtypeOf(toType, fromType, 0);
			else
				return false;
		}
		#endregion
		
		#region Implicit Constant-Expression Conversion
		bool ImplicitConstantExpressionConversion(Expression rr, IType toType)
		{
			if (rr == null || !rr.IsCompileTimeConstant)
				return false;
			// V# 4.0 spec: §6.1.9
			TypeCode fromTypeCode = ReflectionHelper.GetTypeCode(rr.Type);
			TypeCode toTypeCode = ReflectionHelper.GetTypeCode(NullableType.GetUnderlyingType(toType));
			if (fromTypeCode == TypeCode.Int64) {
				long val = (long)rr.ConstantValue;
				return val >= 0 && toTypeCode == TypeCode.UInt64;
			} else if (fromTypeCode == TypeCode.Int32) {
				object cv = rr.ConstantValue;
				if (cv == null)
					return false;
				int val = (int)cv;
				switch (toTypeCode) {
					case TypeCode.SByte:
						return val >= SByte.MinValue && val <= SByte.MaxValue;
					case TypeCode.Byte:
						return val >= Byte.MinValue && val <= Byte.MaxValue;
					case TypeCode.Int16:
						return val >= Int16.MinValue && val <= Int16.MaxValue;
					case TypeCode.UInt16:
						return val >= UInt16.MinValue && val <= UInt16.MaxValue;
					case TypeCode.UInt32:
						return val >= 0;
					case TypeCode.UInt64:
						return val >= 0;
				}
			}
			return false;
		}
		#endregion
		
		#region Conversions involving type parameters
		/// <summary>
		/// Implicit conversions involving type parameters.
		/// </summary>
		bool ImplicitTypeParameterConversion(IType fromType, IType toType)
		{
			if (fromType.Kind != TypeKind.TypeParameter)
				return false; // not a type parameter
			if (fromType.IsReferenceType == true)
				return false; // already handled by ImplicitReferenceConversion
			return IsSubtypeOf(fromType, toType, 0);
		}
		
		Conversion ExplicitTypeParameterConversion(IType fromType, IType toType)
		{
			if (toType.Kind == TypeKind.TypeParameter) {
				// Explicit type parameter conversions that aren't also
				// reference conversions are considered to be unboxing conversions
				if (fromType.Kind == TypeKind.Interface || IsSubtypeOf(toType, fromType, 0))
					return Conversion.UnboxingConversion;
			} else {
				if (fromType.Kind == TypeKind.TypeParameter && toType.Kind == TypeKind.Interface)
					return Conversion.BoxingConversion;
			}
			return Conversion.None;
		}
		#endregion
		
		#region Pointer Conversions
		bool ImplicitPointerConversion(IType fromType, IType toType)
		{
            if (fromType is PointerTypeSpec && toType is PointerTypeSpec && toType.ReflectionName == "Std.Void*")
				return true;
            if (fromType.Kind == TypeKind.Null && toType is PointerTypeSpec)
				return true;
			return false;
		}	
		bool ExplicitPointerConversion(IType fromType, IType toType)
		{
			if (fromType.Kind == TypeKind.Pointer) {
				return toType.Kind == TypeKind.Pointer || IsIntegerType(toType);
			} else {
				return toType.Kind == TypeKind.Pointer && IsIntegerType(fromType);
			}
		}
		
		bool IsIntegerType(IType type)
		{
			TypeCode c = ReflectionHelper.GetTypeCode(type);
			return c >= TypeCode.SByte && c <= TypeCode.UInt64;
		}
		#endregion
		
		#region User-Defined Conversions
		/// <summary>
		/// Gets whether type A is encompassed by type B.
		/// </summary>
		bool IsEncompassedBy(IType a, IType b)
		{
			return a.Kind != TypeKind.Interface && b.Kind != TypeKind.Interface && StandardImplicitConversion(a, b).IsValid;
		}
		
		bool IsEncompassingOrEncompassedBy(IType a, IType b)
		{
			return a.Kind != TypeKind.Interface && b.Kind != TypeKind.Interface
				&& (StandardImplicitConversion(a, b).IsValid || StandardImplicitConversion(b, a).IsValid);
		}

		IType FindMostEncompassedType(IEnumerable<IType> candidates)
		{
			IType best = null;
			foreach (var current in candidates) {
				if (best == null || IsEncompassedBy(current, best))
					best = current;
				else if (!IsEncompassedBy(best, current))
					return null;	// Ambiguous
			}
			return best;
		}

		IType FindMostEncompassingType(IEnumerable<IType> candidates)
		{
			IType best = null;
			foreach (var current in candidates) {
				if (best == null || IsEncompassedBy(best, current))
					best = current;
				else if (!IsEncompassedBy(current, best))
					return null;	// Ambiguous
			}
			return best;
		}

		Conversion SelectOperator(IType mostSpecificSource, IType mostSpecificTarget, IList<OperatorInfo> operators, bool isImplicit, IType source, IType target)
		{
			var selected = operators.Where(op => op.SourceType.Equals(mostSpecificSource) && op.TargetType.Equals(mostSpecificTarget)).ToList();
			if (selected.Count == 0)
				return Conversion.None;

			if (selected.Count == 1)
				return Conversion.UserDefinedConversion(selected[0].Method, isLifted: selected[0].IsLifted, isImplicit: isImplicit, conversionBeforeUserDefinedOperator: ExplicitConversion(source, mostSpecificSource), conversionAfterUserDefinedOperator: ExplicitConversion(mostSpecificTarget, target));

			int nNonLifted = selected.Count(s => !s.IsLifted);
			if (nNonLifted == 1) {
				var op = selected.First(s => !s.IsLifted);
				return Conversion.UserDefinedConversion(op.Method, isLifted: op.IsLifted, isImplicit: isImplicit, conversionBeforeUserDefinedOperator: ExplicitConversion(source, mostSpecificSource), conversionAfterUserDefinedOperator: ExplicitConversion(mostSpecificTarget, target));
			}
			
			return Conversion.UserDefinedConversion(selected[0].Method, isLifted: selected[0].IsLifted, isImplicit: isImplicit, isAmbiguous: true, conversionBeforeUserDefinedOperator: ExplicitConversion(source, mostSpecificSource), conversionAfterUserDefinedOperator: ExplicitConversion(mostSpecificTarget, target));
		}

		Conversion UserDefinedImplicitConversion(Expression fromResult, IType fromType, IType toType)
		{
			// V# 4.0 spec §6.4.4 User-defined implicit conversions
			var operators = GetApplicableConversionOperators(fromResult, fromType, toType, false);

			if (operators.Count > 0) {
				var mostSpecificSource = operators.Any(op => op.SourceType.Equals(fromType)) ? fromType : FindMostEncompassedType(operators.Select(op => op.SourceType));
				if (mostSpecificSource == null)
					return Conversion.UserDefinedConversion(operators[0].Method, isImplicit: true, isLifted: operators[0].IsLifted, isAmbiguous: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
				var mostSpecificTarget = operators.Any(op => op.TargetType.Equals(toType)) ? toType : FindMostEncompassingType(operators.Select(op => op.TargetType));
				if (mostSpecificTarget == null) {
					if (NullableType.IsNullable(toType))
						return UserDefinedImplicitConversion(fromResult, fromType, NullableType.GetUnderlyingType(toType));
					else
						return Conversion.UserDefinedConversion(operators[0].Method, isImplicit: true, isLifted: operators[0].IsLifted, isAmbiguous: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
				}

				var selected = SelectOperator(mostSpecificSource, mostSpecificTarget, operators, true, fromType, toType);
				if (selected != Conversion.None) {
					if (selected.IsLifted && NullableType.IsNullable(toType)) {
						// Prefer A -> B -> B? over A -> A? -> B?
						var other = UserDefinedImplicitConversion(fromResult, fromType, NullableType.GetUnderlyingType(toType));
						if (other != Conversion.None)
							return other;
					}
					return selected;
				}
				else if (NullableType.IsNullable(toType))
					return UserDefinedImplicitConversion(fromResult, fromType, NullableType.GetUnderlyingType(toType));
				else
					return Conversion.None;
			}
			else {
				return Conversion.None;
			}
		}
		
		Conversion UserDefinedExplicitConversion(Expression fromResult, IType fromType, IType toType)
		{
			// V# 4.0 spec §6.4.5 User-defined implicit conversions
			var operators = GetApplicableConversionOperators(fromResult, fromType, toType, true);
			if (operators.Count > 0) {
				IType mostSpecificSource;
				if (operators.Any(op => op.SourceType.Equals(fromType))) {
					mostSpecificSource = fromType;
				} else {
					var operatorsWithSourceEncompassingFromType = operators.Where(op => IsEncompassedBy(fromType, op.SourceType) || ImplicitConstantExpressionConversion(fromResult, NullableType.GetUnderlyingType(op.SourceType))).ToList();
					if (operatorsWithSourceEncompassingFromType.Any())
						mostSpecificSource = FindMostEncompassedType(operatorsWithSourceEncompassingFromType.Select(op => op.SourceType));
					else
						mostSpecificSource = FindMostEncompassingType(operators.Select(op => op.SourceType));
				}
				if (mostSpecificSource == null)
					return Conversion.UserDefinedConversion(operators[0].Method, isImplicit: false, isLifted: operators[0].IsLifted, isAmbiguous: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);

				IType mostSpecificTarget;
				if (operators.Any(op => op.TargetType.Equals(toType)))
					mostSpecificTarget = toType;
				else if (operators.Any(op => IsEncompassedBy(op.TargetType, toType)))
					mostSpecificTarget = FindMostEncompassingType(operators.Where(op => IsEncompassedBy(op.TargetType, toType)).Select(op => op.TargetType));
				else
					mostSpecificTarget = FindMostEncompassedType(operators.Select(op => op.TargetType));
				if (mostSpecificTarget == null) {
					if (NullableType.IsNullable(toType))
						return UserDefinedExplicitConversion(fromResult, fromType, NullableType.GetUnderlyingType(toType));
					else
						return Conversion.UserDefinedConversion(operators[0].Method, isImplicit: false, isLifted: operators[0].IsLifted, isAmbiguous: true, conversionBeforeUserDefinedOperator: Conversion.None, conversionAfterUserDefinedOperator: Conversion.None);
				}

				var selected = SelectOperator(mostSpecificSource, mostSpecificTarget, operators, false, fromType, toType);
				if (selected != Conversion.None) {
					if (selected.IsLifted && NullableType.IsNullable(toType)) {
						// Prefer A -> B -> B? over A -> A? -> B?
						var other = UserDefinedImplicitConversion(fromResult, fromType, NullableType.GetUnderlyingType(toType));
						if (other != Conversion.None)
							return other;
					}
					return selected;
				}
				else if (NullableType.IsNullable(toType))
					return UserDefinedExplicitConversion(fromResult, fromType, NullableType.GetUnderlyingType(toType));
				else if (NullableType.IsNullable(fromType))
					return UserDefinedExplicitConversion(null, NullableType.GetUnderlyingType(fromType), toType);	// A? -> A -> B
				else
					return Conversion.None;
			}
			else {
				return Conversion.None;
			}
		}
		
		class OperatorInfo
		{
			public readonly IMethod Method;
			public readonly IType SourceType;
			public readonly IType TargetType;
			public readonly bool IsLifted;
			
			public OperatorInfo(IMethod method, IType sourceType, IType targetType, bool isLifted)
			{
				this.Method = method;
				this.SourceType = sourceType;
				this.TargetType = targetType;
				this.IsLifted = isLifted;
			}
		}
		
		List<OperatorInfo> GetApplicableConversionOperators(Expression fromResult, IType fromType, IType toType, bool isExplicit)
		{
			// Find the candidate operators:
			Predicate<IUnresolvedMethod> opFilter;
			if (isExplicit)
				opFilter = m => m.IsStatic && m.IsOperator && (m.Name == "op_Explicit" || m.Name == "op_Implicit") && m.Parameters.Count == 1;
			else
				opFilter = m => m.IsStatic && m.IsOperator && m.Name == "op_Implicit" && m.Parameters.Count == 1;
			
			var operators = NullableType.GetUnderlyingType(fromType).GetMethods(opFilter)
				.Concat(NullableType.GetUnderlyingType(toType).GetMethods(opFilter)).Distinct();
			// Determine whether one of them is applicable:
			List<OperatorInfo> result = new List<OperatorInfo>();
			foreach (IMethod op in operators) {
				IType sourceType = op.Parameters[0].Type;
				IType targetType = op.ReturnType;
				// Try if the operator is applicable:
				bool isApplicable;
				if (isExplicit) {
					isApplicable = (IsEncompassingOrEncompassedBy(fromType, sourceType) || ImplicitConstantExpressionConversion(fromResult, sourceType))
						&& IsEncompassingOrEncompassedBy(targetType, toType);
				} else {
					isApplicable = (IsEncompassedBy(fromType, sourceType) || ImplicitConstantExpressionConversion(fromResult, sourceType))
						&& IsEncompassedBy(targetType, toType);
				}
				// Try if the operator is applicable in lifted form:
				if (isApplicable) {
					result.Add(new OperatorInfo(op, sourceType, targetType, false));
				}
				if (NullableType.IsNonNullableValueType(sourceType)) {
					// An operator can be applicable in both lifted and non-lifted form in case of explicit conversions
					IType liftedSourceType = NullableType.Create(compilation, sourceType);
					IType liftedTargetType = NullableType.IsNonNullableValueType(targetType) ? NullableType.Create(compilation, targetType) : targetType;
					if (isExplicit) {
						isApplicable = IsEncompassingOrEncompassedBy(fromType, liftedSourceType)
							&& IsEncompassingOrEncompassedBy(liftedTargetType, toType);
					} else {
						isApplicable = IsEncompassedBy(fromType, liftedSourceType) && IsEncompassedBy(liftedTargetType, toType);
					}

					if (isApplicable) {
						result.Add(new OperatorInfo(op, liftedSourceType, liftedTargetType, true));
					}
				}
			}
			return result;
		}
		#endregion
		
		#region AnonymousFunctionConversion
		Conversion AnonymousFunctionConversion(Expression expression, IType toType)
		{
			// V# 5.0 spec §6.5 Anonymous function conversions
			LambdaExpression f = expression as LambdaExpression;
			if (f == null)
				return Conversion.None;
	
			IMethod d = toType.GetDelegateInvokeMethod();
			if (d == null)
				return Conversion.None;
			
			IType[] dParamTypes = new IType[d.Parameters.Count];
			for (int i = 0; i < dParamTypes.Length; i++) {
				dParamTypes[i] = d.Parameters[i].Type;
			}
			IType dReturnType = d.ReturnType;
			
			if (f.HasParameterList) {
				// If F contains an anonymous-function-signature, then D and F have the same number of parameters.
				if (d.Parameters.Count != f.Parameters.Count)
					return Conversion.None;
				
				if (f.IsImplicitlyTyped) {
					// If F has an implicitly typed parameter list, D has no ref or out parameters.
					foreach (IParameter p in d.Parameters) {
						if (p.IsOut || p.IsRef)
							return Conversion.None;
					}
				} else {
					// If F has an explicitly typed parameter list, each parameter in D has the same type
					// and modifiers as the corresponding parameter in F.
					for (int i = 0; i < f.Parameters.Count; i++) {
						IParameter pD = d.Parameters[i];
						IParameter pF = f.Parameters[i];
						if (pD.IsRef != pF.IsRef || pD.IsOut != pF.IsOut)
							return Conversion.None;
						if (!IdentityConversion(dParamTypes[i], pF.Type))
							return Conversion.None;
					}
				}
			} else {
				// If F does not contain an anonymous-function-signature, then D may have zero or more parameters of any
				// type, as long as no parameter of D has the out parameter modifier.
				foreach (IParameter p in d.Parameters) {
					if (p.IsOut)
						return Conversion.None;
				}
			}
			
			return f.IsValid(dParamTypes, dReturnType, this);
		}

		#endregion
		
	
		
		#region BetterConversion
		/// <summary>
		/// Gets the better conversion (V# 4.0 spec, §7.5.3.3)
		/// </summary>
		/// <returns>0 = neither is better; 1 = t1 is better; 2 = t2 is better</returns>
		public int BetterConversion(Expression expression, IType t1, IType t2)
		{
			LambdaExpression lambda = expression as LambdaExpression;
			if (lambda != null) {
				
				IMethod m1 = t1.GetDelegateInvokeMethod();
				IMethod m2 = t2.GetDelegateInvokeMethod();
				if (m1 == null || m2 == null)
					return 0;
				if (m1.Parameters.Count != m2.Parameters.Count)
					return 0;
				IType[] parameterTypes = new IType[m1.Parameters.Count];
				for (int i = 0; i < parameterTypes.Length; i++) {
					parameterTypes[i] = m1.Parameters[i].Type;
					if (!parameterTypes[i].Equals(m2.Parameters[i].Type))
						return 0;
				}
				if (lambda.HasParameterList && parameterTypes.Length != lambda.Parameters.Count)
					return 0;
				
				IType ret1 = m1.ReturnType;
				IType ret2 = m2.ReturnType;
				if (ret1.Kind == TypeKind.Void && ret2.Kind != TypeKind.Void)
					return 2;
				if (ret1.Kind != TypeKind.Void && ret2.Kind == TypeKind.Void)
					return 1;
				
				IType inferredRet = lambda.GetInferredReturnType(parameterTypes);
				int r = BetterConversion(inferredRet, ret1, ret2);
				
				return r;
			} else {
				return BetterConversion(expression.Type, t1, t2);
			}
		}
		
		
		
		/// <summary>
		/// Gets the better conversion (V# 4.0 spec, §7.5.3.4)
		/// </summary>
		/// <returns>0 = neither is better; 1 = t1 is better; 2 = t2 is better</returns>
		public int BetterConversion(IType s, IType t1, IType t2)
		{
			bool ident1 = IdentityConversion(s, t1);
			bool ident2 = IdentityConversion(s, t2);
			if (ident1 && !ident2)
				return 1;
			if (ident2 && !ident1)
				return 2;
			return BetterConversionTarget(t1, t2);
		}
		
		/// <summary>
		/// Gets the better conversion target (V# 4.0 spec, §7.5.3.5)
		/// </summary>
		/// <returns>0 = neither is better; 1 = t1 is better; 2 = t2 is better</returns>
		int BetterConversionTarget(IType t1, IType t2)
		{
			bool t1To2 = ImplicitConversion(t1, t2).IsValid;
			bool t2To1 = ImplicitConversion(t2, t1).IsValid;
			if (t1To2 && !t2To1)
				return 1;
			if (t2To1 && !t1To2)
				return 2;
			TypeCode t1Code = ReflectionHelper.GetTypeCode(t1);
			TypeCode t2Code = ReflectionHelper.GetTypeCode(t2);
			if (IsBetterIntegralType(t1Code, t2Code))
				return 1;
			if (IsBetterIntegralType(t2Code, t1Code))
				return 2;
			return 0;
		}
		
		bool IsBetterIntegralType(TypeCode t1, TypeCode t2)
		{
			// signed types are better than unsigned types
			switch (t1) {
				case TypeCode.SByte:
					return t2 == TypeCode.Byte || t2 == TypeCode.UInt16 || t2 == TypeCode.UInt32 || t2 == TypeCode.UInt64;
				case TypeCode.Int16:
					return t2 == TypeCode.UInt16 || t2 == TypeCode.UInt32 || t2 == TypeCode.UInt64;
				case TypeCode.Int32:
					return t2 == TypeCode.UInt32 || t2 == TypeCode.UInt64;
				case TypeCode.Int64:
					return t2 == TypeCode.UInt64;
				default:
					return false;
			}
		}
		#endregion
	}
}
