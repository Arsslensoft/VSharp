namespace VSC.AST
{
    public class Let : ARangeVariableQueryClause
    {
        public Let(QueryBlock block, RangeVariable identifier, Expression expr, Location loc)
            : base(block, identifier, expr, loc)
        {
        }

       

        protected override string MethodName
        {
            get { return "Select"; }
        }


    }
}