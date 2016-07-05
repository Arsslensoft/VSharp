namespace VSC.AST
{
    public class SelectMany : ARangeVariableQueryClause
    {
        public SelectMany(QueryBlock block, RangeVariable identifier, Expression expr, Location loc)
            : base(block, identifier, expr, loc)
        {
        }


        protected override string MethodName
        {
            get { return "SelectMany"; }
        }

     
    }
}