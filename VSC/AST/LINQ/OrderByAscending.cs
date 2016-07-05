namespace VSC.AST
{
    public class OrderByAscending : AQueryClause
    {
        public OrderByAscending(QueryBlock block, Expression expr)
            : base(block, expr, expr.Location)
        {
        }

        protected override string MethodName
        {
            get { return "OrderBy"; }
        }

      
    }
}