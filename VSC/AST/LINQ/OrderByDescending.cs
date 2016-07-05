namespace VSC.AST
{
    public class OrderByDescending : AQueryClause
    {
        public OrderByDescending(QueryBlock block, Expression expr)
            : base(block, expr, expr.Location)
        {
        }

        protected override string MethodName
        {
            get { return "OrderByDescending"; }
        }

      
    }
}