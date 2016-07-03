using System;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Cache for KnownTypeReferences.
	/// </summary>
	sealed class KnownTypeCache
	{
		readonly ICompilation compilation;
		readonly IType[] knownTypes = new IType[KnownTypeReference.KnownTypeCodeCount];
		
		public KnownTypeCache(ICompilation compilation)
		{
			this.compilation = compilation;
		}
		
		public IType FindType(KnownTypeCode typeCode)
		{
			IType type = LazyInit.VolatileRead(ref knownTypes[(int)typeCode]);
			if (type != null) {
				return type;
			}
			return LazyInit.GetOrSet(ref knownTypes[(int)typeCode], SearchType(typeCode));
		}
		
		IType SearchType(KnownTypeCode typeCode)
		{
			KnownTypeReference typeRef = KnownTypeReference.Get(typeCode);
			if (typeRef == null)
				return SpecialTypeSpec.UnknownType;
			var typeName = new TopLevelTypeName(typeRef.Namespace, typeRef.Name, typeRef.TypeParameterCount);
			foreach (IAssembly asm in compilation.Assemblies) {
				var typeDef = asm.GetTypeDefinition(typeName);
				if (typeDef != null)
					return typeDef;
			}
			return new UnknownTypeSpec(typeName);
		}
	}
}
