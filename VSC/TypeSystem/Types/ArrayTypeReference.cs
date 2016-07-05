using System;

namespace VSC.TypeSystem
{
    [Serializable]
    public sealed class ArrayTypeReference : ITypeReference, ISupportsInterning
    {
        readonly ITypeReference elementType;
        readonly int dimensions;
		
        public ArrayTypeReference(ITypeReference elementType, int dimensions = 1)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            if (dimensions <= 0)
                throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
            this.elementType = elementType;
            this.dimensions = dimensions;
        }
		
        public ITypeReference ElementType {
            get { return elementType; }
        }
		
        public int Dimensions {
            get { return dimensions; }
        }
		
        public IType Resolve(ITypeResolveContext context)
        {
            return new ArrayType(context.Compilation, elementType.Resolve(context), dimensions);
        }
		
        public override string ToString()
        {
            return elementType.ToString() + "[" + new string(',', dimensions - 1) + "]";
        }
		
        int ISupportsInterning.GetHashCodeForInterning()
        {
            return elementType.GetHashCode() ^ dimensions;
        }
		
        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            ArrayTypeReference o = other as ArrayTypeReference;
            return o != null && elementType == o.elementType && dimensions == o.dimensions;
        }
    }
}