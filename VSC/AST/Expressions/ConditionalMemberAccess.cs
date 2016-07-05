namespace VSC.AST
{
    public class ConditionalMemberAccess : MemberAccess
    {
        public ConditionalMemberAccess(Expression expr, string identifier, TypeArguments args, Location loc)
            : base(expr, identifier, args, loc, TypeSystem.Resolver.NameLookupMode.Expression)
        {
        }
    }
}