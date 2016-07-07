using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Static helper methods for working with nullable types.
	/// </summary>
	public static class NullableType
	{
		/// <summary>
		/// Gets whether the specified type is a nullable type.
		/// </summary>
		public static bool IsNullable(IType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
            ParameterizedTypeSpec pt = type as ParameterizedTypeSpec;
			return pt != null && pt.TypeParameterCount == 1 && pt.GetDefinition().KnownTypeCode == KnownTypeCode.NullableOfT;
		}
		
		public static bool IsNonNullableValueType(IType type)
		{
			return type.IsReferenceType == false && !IsNullable(type);
		}
		
		/// <summary>
		/// Returns the element type, if <paramref name="type"/> is a nullable type.
		/// Otherwise, returns the type itself.
		/// </summary>
		public static IType GetUnderlyingType(IType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
            ParameterizedTypeSpec pt = type as ParameterizedTypeSpec;
            if (pt != null && pt.TypeParameterCount == 1 && pt.FullName == "Std.Nullable")
				return pt.GetTypeArgument(0);
			else
				return type;
		}
		
		/// <summary>
		/// Creates a nullable type.
		/// </summary>
		public static IType Create(ICompilation compilation, IType elementType)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			
			IType nullableType = compilation.FindType(KnownTypeCode.NullableOfT);
			ITypeDefinition nullableTypeDef = nullableType.GetDefinition();
			if (nullableTypeDef != null)
                return new ParameterizedTypeSpec(nullableTypeDef, new[] { elementType });
			else
				return nullableType;
		}
		
		/// <summary>
		/// Creates a nullable type reference.
		/// </summary>
		public static ParameterizedTypeReference Create(ITypeReference elementType)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			return new ParameterizedTypeReference(KnownTypeReference.NullableOfT, new [] { elementType });
		}
	}
}
