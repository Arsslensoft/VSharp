using System;

namespace VSC.TypeSystem.Implementation
{
    public sealed class NamespaceReference : ISymbolReference
    {
        IAssemblyReference assemblyReference;
        string fullName;
		
        public NamespaceReference(IAssemblyReference assemblyReference, string fullName)
        {
            if (assemblyReference == null)
                throw new ArgumentNullException("assemblyReference");
            this.assemblyReference = assemblyReference;
            this.fullName = fullName;
        }
		
        public ISymbol Resolve(ITypeResolveContext context)
        {
            IAssembly assembly = assemblyReference.Resolve(context);
            INamespace parent = assembly.RootNamespace;
			
            string[] parts = fullName.Split('.');
			
            int i = 0;
            while (i < parts.Length && parent != null) {
                parent = parent.GetChildNamespace(parts[i]);
                i++;
            }
			
            return parent;
        }
    }
}