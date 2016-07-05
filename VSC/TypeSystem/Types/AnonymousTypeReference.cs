using System;

namespace VSC.TypeSystem
{
    /// <summary>
    /// Anonymous type reference.
    /// </summary>
    [Serializable]
    public class AnonymousTypeReference : ITypeReference
    {
        readonly IUnresolvedProperty[] unresolvedProperties;
		
        public AnonymousTypeReference(IUnresolvedProperty[] properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");
            this.unresolvedProperties = properties;
        }
		
        public IType Resolve(ITypeResolveContext context)
        {
            return new AnonymousTypeSpec(context.Compilation, unresolvedProperties);
        }
    }
}