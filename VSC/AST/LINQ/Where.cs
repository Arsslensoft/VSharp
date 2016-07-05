namespace VSC.AST
{
    public class Where : AQueryClause
    {
        public Where(QueryBlock block, Expression expr, Location loc)
            : base(block, expr, loc)
        {
        }

        protected override string MethodName
        {
            get { return "Where"; }
        }

    }
}