using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VSC.TypeSystem
{
    /// <summary>
    /// ParameterizedTypeReference is a reference to generic class that specifies the type parameters.
    /// Example: List&lt;string&gt;
    /// </summary>
    [Serializable]
    public sealed class ParameterizedTypeReference : ITypeReference, ISupportsInterning
    {
        readonly ITypeReference genericType;
        readonly ITypeReference[] typeArguments;
		
        public ParameterizedTypeReference(ITypeReference genericType, IEnumerable<ITypeReference> typeArguments)
        {
            if (genericType == null)
                throw new ArgumentNullException("genericType");
            if (typeArguments == null)
                throw new ArgumentNullException("typeArguments");
            this.genericType = genericType;
            this.typeArguments = typeArguments.ToArray();
            for (int i = 0; i < this.typeArguments.Length; i++) {
                if (this.typeArguments[i] == null)
                    throw new ArgumentNullException("typeArguments[" + i + "]");
            }
        }
		
        public ITypeReference GenericType {
            get { return genericType; }
        }
		
        public ReadOnlyCollection<ITypeReference> TypeArguments {
            get {
                return Array.AsReadOnly(typeArguments);
            }
        }
		
        public IType Resolve(ITypeResolveContext context)
        {
            IType baseType = genericType.Resolve(context);
            ITypeDefinition baseTypeDef = baseType.GetDefinition();
            if (baseTypeDef == null)
                return baseType;
            int tpc = baseTypeDef.TypeParameterCount;
            if (tpc == 0)
                return baseTypeDef;
            IType[] resolvedTypes = new IType[tpc];
            for (int i = 0; i < resolvedTypes.Length; i++) {
                if (i < typeArguments.Length)
                    resolvedTypes[i] = typeArguments[i].Resolve(context);
                else
                    resolvedTypes[i] = SpecialTypeSpec.UnknownType;
            }
            return new ParameterizedTypeSpec(baseTypeDef, resolvedTypes);
        }
		
        public override string ToString()
        {
            StringBuilder b = new StringBuilder(genericType.ToString());
            b.Append('[');
            for (int i = 0; i < typeArguments.Length; i++) {
                if (i > 0)
                    b.Append(',');
                b.Append('[');
                b.Append(typeArguments[i].ToString());
                b.Append(']');
            }
            b.Append(']');
            return b.ToString();
        }
		
        int ISupportsInterning.GetHashCodeForInterning()
        {
            int hashCode = genericType.GetHashCode();
            unchecked {
                foreach (ITypeReference t in typeArguments) {
                    hashCode *= 27;
                    hashCode += t.GetHashCode();
                }
            }
            return hashCode;
        }
		
        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            ParameterizedTypeReference o = other as ParameterizedTypeReference;
            if (o != null && genericType == o.genericType && typeArguments.Length == o.typeArguments.Length) {
                for (int i = 0; i < typeArguments.Length; i++) {
                    if (typeArguments[i] != o.typeArguments[i])
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}