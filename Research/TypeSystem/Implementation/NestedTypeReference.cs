using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Type reference used to reference nested types.
	/// </summary>
	[Serializable]
	public sealed class NestedTypeReference : ITypeReference, ISymbolReference, ISupportsInterning
	{
		readonly ITypeReference declaringTypeRef;
		readonly string name;
		readonly int additionalTypeParameterCount;
		
		/// <summary>
		/// Creates a new NestedTypeReference.
		/// </summary>
		/// <param name="declaringTypeRef">Reference to the declaring type.</param>
		/// <param name="name">Name of the nested class</param>
		/// <param name="additionalTypeParameterCount">Number of type parameters on the inner class (without type parameters on baseTypeRef)</param>
		/// <remarks>
		/// <paramref name="declaringTypeRef"/> must be exactly the (unbound) declaring type, not a derived type, not a parameterized type.
		/// NestedTypeReference thus always resolves to a type definition, never to (partially) parameterized types.
		/// </remarks>
		public NestedTypeReference(ITypeReference declaringTypeRef, string name, int additionalTypeParameterCount)
		{
			if (declaringTypeRef == null)
				throw new ArgumentNullException("declaringTypeRef");
			if (name == null)
				throw new ArgumentNullException("name");
			this.declaringTypeRef = declaringTypeRef;
			this.name = name;
			this.additionalTypeParameterCount = additionalTypeParameterCount;
		}
		
		public ITypeReference DeclaringTypeReference {
			get { return declaringTypeRef; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public int AdditionalTypeParameterCount {
			get { return additionalTypeParameterCount; }
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			ITypeDefinition declaringType = declaringTypeRef.Resolve(context) as ITypeDefinition;
			if (declaringType != null) {
				int tpc = declaringType.TypeParameterCount;
				foreach (IType type in declaringType.NestedTypes) {
					if (type.Name == name && type.TypeParameterCount == tpc + additionalTypeParameterCount)
						return type;
				}
			}
			return new UnknownTypeSpec(null, name, additionalTypeParameterCount);
		}
		
		ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
		{
			var type = Resolve(context);
			if (type is ITypeDefinition)
				return (ISymbol)type;
			return null;
		}
		
		public override string ToString()
		{
			if (additionalTypeParameterCount == 0)
				return declaringTypeRef + "+" + name;
			else
				return declaringTypeRef + "+" + name + "`" + additionalTypeParameterCount;
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return declaringTypeRef.GetHashCode() ^ name.GetHashCode() ^ additionalTypeParameterCount;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			NestedTypeReference o = other as NestedTypeReference;
			return o != null && declaringTypeRef == o.declaringTypeRef && name == o.name && additionalTypeParameterCount == o.additionalTypeParameterCount;
		}
	}
}
