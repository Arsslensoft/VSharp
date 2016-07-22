using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
    /// <summary>
    /// Represents an ambiguous field/property/event access.
    /// </summary>
    public class AmbiguousMemberExpression : MemberExpressionStatement
    {
        public AmbiguousMemberExpression(Expression targetResult, IMember member) : base(targetResult, member)
        {
        }
		
        public override bool IsError {
            get { return true; }
        }
    }
}