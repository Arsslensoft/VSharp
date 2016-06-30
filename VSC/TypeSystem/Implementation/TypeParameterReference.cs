using System;
using System.Globalization;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	[Serializable]
	public sealed class TypeParameterReference : ITypeReference, ISymbolReference
	{
		static readonly TypeParameterReference[] classTypeParameterReferences = new TypeParameterReference[8];
		static readonly TypeParameterReference[] methodTypeParameterReferences = new TypeParameterReference[8];
		
		/// <summary>
		/// Creates a type parameter reference.
		/// For common type parameter references, this method may return a shared instance.
		/// </summary>
		public static TypeParameterReference Create(SymbolKind ownerType, int index)
		{
			if (index >= 0 && index < 8 && (ownerType == SymbolKind.TypeDefinition || ownerType == SymbolKind.Method)) {
				TypeParameterReference[] arr = (ownerType == SymbolKind.TypeDefinition) ? classTypeParameterReferences : methodTypeParameterReferences;
				TypeParameterReference result = LazyInit.VolatileRead(ref arr[index]);
				if (result == null) {
					result = LazyInit.GetOrSet(ref arr[index], new TypeParameterReference(ownerType, index));
				}
				return result;
			} else {
				return new TypeParameterReference(ownerType, index);
			}
		}
		
		readonly SymbolKind ownerType;
		readonly int index;

		public int Index {
			get {
				return index;
			}
		}

		public TypeParameterReference(SymbolKind ownerType, int index)
		{
			this.ownerType = ownerType;
			this.index = index;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			if (ownerType == SymbolKind.Method) {
				IMethod method = context.CurrentMember as IMethod;
				if (method != null && index < method.TypeParameters.Count) {
					return method.TypeParameters[index];
				}
				return DummyTypeParameter.GetMethodTypeParameter(index);
			} else if (ownerType == SymbolKind.TypeDefinition) {
				ITypeDefinition typeDef = context.CurrentTypeDefinition;
				if (typeDef != null && index < typeDef.TypeParameters.Count) {
					return typeDef.TypeParameters[index];
				}
				return DummyTypeParameter.GetClassTypeParameter(index);
			} else {
				return SpecialTypeSpec.UnknownType;
			}
		}
		
		ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
		{
			return Resolve(context) as ISymbol;
		}
		
		public override string ToString()
		{
			if (ownerType == SymbolKind.Method)
				return "!!" + index.ToString(CultureInfo.InvariantCulture);
			else
				return "!" + index.ToString(CultureInfo.InvariantCulture);
		}
	}
}
