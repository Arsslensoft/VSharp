using System;

namespace VSC.TypeSystem.Implementation
{
    public sealed class OwnedTypeParameterReference : ISymbolReference
    {
        ISymbolReference owner;
        int index;
		
        public OwnedTypeParameterReference(ISymbolReference owner, int index)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            this.owner = owner;
            this.index = index;
        }
		
        public ISymbol Resolve(ITypeResolveContext context)
        {
            var entity = owner.Resolve(context) as IEntity;
            if (entity is ITypeDefinition)
                return ((ITypeDefinition)entity).TypeParameters[index];
            if (entity is IMethod)
                return ((IMethod)entity).TypeParameters[index];
            return null;
        }
    }
}