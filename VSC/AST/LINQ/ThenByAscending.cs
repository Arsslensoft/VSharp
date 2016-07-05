namespace VSC.AST
{
    public class ThenByAscending : OrderByAscending
    {
        public ThenByAscending(QueryBlock block, Expression expr)
            : base(block, expr)
        {
        }

        protected override string MethodName
        {
            get { return "ThenBy"; }
        }

     
    }
}