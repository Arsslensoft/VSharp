using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
    /// <summary>
    /// Represents an ambiguous field/property/event access.
    /// </summary>
    public class AmbiguousMemberResolveResult : MemberResolveResult
    {
        public AmbiguousMemberResolveResult(ResolveResult targetResult, IMember member) : base(targetResult, member)
        {
        }
		
        public override bool IsError {
            get { return true; }
        }
    }
}