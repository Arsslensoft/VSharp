using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
    [Serializable]
    public sealed class FullTypeNameComparer : IEqualityComparer<FullTypeName>
    {
        public static readonly FullTypeNameComparer Ordinal = new FullTypeNameComparer(StringComparer.Ordinal);
        public static readonly FullTypeNameComparer OrdinalIgnoreCase = new FullTypeNameComparer(StringComparer.OrdinalIgnoreCase);
		
        public readonly StringComparer NameComparer;
		
        public FullTypeNameComparer(StringComparer nameComparer)
        {
            this.NameComparer = nameComparer;
        }
		
        public bool Equals(FullTypeName x, FullTypeName y)
        {
            if (x.NestingLevel != y.NestingLevel)
                return false;
            TopLevelTypeName topX = x.TopLevelTypeName;
            TopLevelTypeName topY = y.TopLevelTypeName;
            if (topX.TypeParameterCount == topY.TypeParameterCount
                && NameComparer.Equals(topX.Name, topY.Name)
                && NameComparer.Equals(topX.Namespace, topY.Namespace))
            {
                for (int i = 0; i < x.NestingLevel; i++) {
                    if (x.GetNestedTypeAdditionalTypeParameterCount(i) != y.GetNestedTypeAdditionalTypeParameterCount(i))
                        return false;
                    if (!NameComparer.Equals(x.GetNestedTypeName(i), y.GetNestedTypeName(i)))
                        return false;
                }
                return true;
            }
            return false;
        }
		
        public int GetHashCode(FullTypeName obj)
        {
            TopLevelTypeName top = obj.TopLevelTypeName;
            int hash = NameComparer.GetHashCode(top.Name) ^ NameComparer.GetHashCode(top.Namespace) ^ top.TypeParameterCount;
            unchecked {
                for (int i = 0; i < obj.NestingLevel; i++) {
                    hash *= 31;
                    hash += NameComparer.GetHashCode(obj.Name) ^ obj.TypeParameterCount;
                }
            }
            return hash;
        }
    }
}