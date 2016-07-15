using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// ResolveScope context represents the minimal mscorlib required for evaluating constants.
	/// This contains all known types (<see cref="KnownTypeCode"/>) and no other types.
	/// </summary>
	public sealed class MinimalCorlib : UnresolvedAssemblySpec
	{
		static readonly Lazy<MinimalCorlib> instance = new Lazy<MinimalCorlib>(() => new MinimalCorlib());
		
		public static MinimalCorlib Instance {
			get { return instance.Value; }
		}
		
		public ICompilation CreateCompilation()
		{
			return new SimpleCompilation(new DefaultSolutionSnapshot(), this);
		}
		
		private MinimalCorlib() : base("corlib")
		{
			var types = new TypeDefinitionCore[KnownTypeReference.KnownTypeCodeCount];
			for (int i = 0; i < types.Length; i++) {
				var typeRef = KnownTypeReference.Get((KnownTypeCode)i);
				if (typeRef != null) {
					types[i] = new TypeDefinitionCore(typeRef.Namespace, typeRef.Name);
					for (int j = 0; j < typeRef.TypeParameterCount; j++) {
						types[i].TypeParameters.Add(new UnresolvedTypeParameterSpec(SymbolKind.TypeDefinition, j, VSC.Location.Null));
					}
					AddTypeDefinition(types[i]);
				}
			}
			for (int i = 0; i < types.Length; i++) {
				var typeRef = KnownTypeReference.Get((KnownTypeCode)i);
				if (typeRef != null && typeRef.baseType != KnownTypeCode.None) {
					types[i].BaseTypes.Add(types[(int)typeRef.baseType]);
					if (typeRef.baseType == KnownTypeCode.ValueType && i != (int)KnownTypeCode.Enum) {
						types[i].Kind = TypeKind.Struct;
					}
				}
			}
			Freeze();
		}
	}
}
