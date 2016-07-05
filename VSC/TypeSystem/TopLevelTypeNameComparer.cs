using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
    [Serializable]
    public sealed class TopLevelTypeNameComparer : IEqualityComparer<TopLevelTypeName>
    {
        public static readonly TopLevelTypeNameComparer Ordinal = new TopLevelTypeNameComparer(StringComparer.Ordinal);
        public static readonly TopLevelTypeNameComparer OrdinalIgnoreCase = new TopLevelTypeNameComparer(StringComparer.OrdinalIgnoreCase);
		
        public readonly StringComparer NameComparer;
		
        public TopLevelTypeNameComparer(StringComparer nameComparer)
        {
            this.NameComparer = nameComparer;
        }
		
        public bool Equals(TopLevelTypeName x, TopLevelTypeName y)
        {
            return x.TypeParameterCount == y.TypeParameterCount
                   && NameComparer.Equals(x.Name, y.Name)
                   && NameComparer.Equals(x.Namespace, y.Namespace);
        }
		
        public int GetHashCode(TopLevelTypeName obj)
        {
            return NameComparer.GetHashCode(obj.Name) ^ NameComparer.GetHashCode(obj.Namespace) ^ obj.TypeParameterCount;
        }
    }
}