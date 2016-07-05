using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
    /// <summary>
    /// Compares member signatures.
    /// </summary>
    /// <remarks>
    /// This comparer checks for equal short name, equal type parameter count, and equal parameter types (using ParameterListComparer).
    /// </remarks>
    public sealed class SignatureComparer : IEqualityComparer<IMember>
    {
        StringComparer nameComparer;
		
        public SignatureComparer(StringComparer nameComparer)
        {
            if (nameComparer == null)
                throw new ArgumentNullException("nameComparer");
            this.nameComparer = nameComparer;
        }
		
        /// <summary>
        /// Gets a signature comparer that uses an ordinal comparison for the member name.
        /// </summary>
        public static readonly SignatureComparer Ordinal = new SignatureComparer(StringComparer.Ordinal);
		
        public bool Equals(IMember x, IMember y)
        {
            if (x == y)
                return true;
            if (x == null || y == null || x.SymbolKind != y.SymbolKind || !nameComparer.Equals(x.Name, y.Name))
                return false;
            IParameterizedMember px = x as IParameterizedMember;
            IParameterizedMember py = y as IParameterizedMember;
            if (px != null && py != null) {
                IMethod mx = x as IMethod;
                IMethod my = y as IMethod;
                if (mx != null && my != null && mx.TypeParameters.Count != my.TypeParameters.Count)
                    return false;
                return ParameterListComparer.Instance.Equals(px.Parameters, py.Parameters);
            } else {
                return true;
            }
        }
		
        public int GetHashCode(IMember obj)
        {
            unchecked {
                int hash = (int)obj.SymbolKind * 33 + nameComparer.GetHashCode(obj.Name);
                IParameterizedMember pm = obj as IParameterizedMember;
                if (pm != null) {
                    hash *= 27;
                    hash += ParameterListComparer.Instance.GetHashCode(pm.Parameters);
                    IMethod m = pm as IMethod;
                    if (m != null)
                        hash += m.TypeParameters.Count;
                }
                return hash;
            }
        }
    }
}