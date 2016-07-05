using System;

namespace VSC.TypeSystem
{
    [Serializable]
    public sealed class PointerTypeReference : ITypeReference, ISupportsInterning
    {
        readonly ITypeReference elementType;
		
        public PointerTypeReference(ITypeReference elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            this.elementType = elementType;
        }
		
        public ITypeReference ElementType {
            get { return elementType; }
        }
		
        public IType Resolve(ITypeResolveContext context)
        {
            return new PointerTypeSpec(elementType.Resolve(context));
        }
		
        public override string ToString()
        {
            return elementType.ToString() + "*";
        }
		
        int ISupportsInterning.GetHashCodeForInterning()
        {
            return elementType.GetHashCode() ^ 91725812;
        }
		
        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            PointerTypeReference o = other as PointerTypeReference;
            return o != null && this.elementType == o.elementType;
        }
    }
}