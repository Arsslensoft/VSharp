namespace VSC.AST
{
    public class ThenByDescending : OrderByDescending
    {
        public ThenByDescending(QueryBlock block, Expression expr)
            : base(block, expr)
        {
        }

        protected override string MethodName
        {
            get { return "ThenByDescending"; }
        }

       
    }
}