using System;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.TypeSystem
{
	[Serializable]
	public class VSharpUnresolvedTypeDefinition : UnresolvedTypeDefinitionSpec
	{
		readonly UsingScope usingScope;
		
		public VSharpUnresolvedTypeDefinition(UsingScope usingScope, string name)
			: base(usingScope.NamespaceName, name)
		{
			this.usingScope = usingScope;
			this.AddDefaultConstructorIfRequired = true;
            GlobalTypeDefinition = false;
		}
		
		public VSharpUnresolvedTypeDefinition(VSharpUnresolvedTypeDefinition declaringTypeDefinition, string name)
			: base(declaringTypeDefinition, name)
		{
			this.usingScope = declaringTypeDefinition.usingScope;
            this.AddDefaultConstructorIfRequired = true; GlobalTypeDefinition = false;
		}
        public bool GlobalTypeDefinition { get; set; }
		public override ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
		{
			return new VSharpTypeResolveContext(parentContext.CurrentAssembly, usingScope.Resolve(parentContext.Compilation), parentContext.CurrentTypeDefinition);
		}
	}
}
