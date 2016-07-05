namespace VSC.TypeSystem.Implementation
{
    public sealed class MergedNamespaceReference : ISymbolReference
    {
        string externAlias;
        string fullName;
		
        public MergedNamespaceReference(string externAlias, string fullName)
        {
            this.externAlias = externAlias;
            this.fullName = fullName;
        }
		
        public ISymbol Resolve(ITypeResolveContext context)
        {
            string[] parts = fullName.Split('.');
            INamespace parent = context.Compilation.GetNamespaceForExternAlias(externAlias);
			
            int i = 0;
            while (i < parts.Length && parent != null) {
                parent = parent.GetChildNamespace(parts[i]);
                i++;
            }
			
            return parent;
        }
    }
}