using System;

namespace VSC.TypeSystem
{
    [Serializable]
    public sealed class ByReferenceTypeReference : ITypeReference, ISupportsInterning
    {
        readonly ITypeReference elementType;
		
        public ByReferenceTypeReference(ITypeReference elementType)
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
            return new ByReferenceType(elementType.Resolve(context));
        }
		
        public override string ToString()
        {
            return elementType.ToString() + "&";
        }
		
        int ISupportsInterning.GetHashCodeForInterning()
        {
            return elementType.GetHashCode() ^ 91725814;
        }
		
        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            ByReferenceTypeReference brt = other as ByReferenceTypeReference;
            return brt != null && this.elementType == brt.elementType;
        }
    }
}