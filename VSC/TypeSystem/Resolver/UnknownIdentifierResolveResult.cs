using System.Globalization;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
    /// <summary>
    /// Represents an unknown identifier.
    /// </summary>
    public class UnknownIdentifierResolveResult : ResolveResult
    {
        readonly string identifier;
        readonly int typeArgumentCount;
		
        public UnknownIdentifierResolveResult(string identifier, int typeArgumentCount = 0)
            : base(SpecialTypeSpec.UnknownType)
        {
            this.identifier = identifier;
            this.typeArgumentCount = typeArgumentCount;
        }
		
        public string Identifier {
            get { return identifier; }
        }
		
        public int TypeArgumentCount {
            get { return typeArgumentCount; }
        }
		
        public override bool IsError {
            get { return true; }
        }
		
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, identifier);
        }
    }
}