namespace VSC.AST
{
    public class Select : AQueryClause
    {
        public Select(QueryBlock block, Expression expr, Location loc)
            : base(block, expr, loc)
        {
        }

      
        protected override string MethodName
        {
            get { return "Select"; }
        }

       

    }
}