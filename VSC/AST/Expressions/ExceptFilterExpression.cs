namespace VSC.AST
{
    public class ExceptFilterExpression : BooleanExpression
    {
        public ExceptFilterExpression(Expression expr, Location loc)
            : base(expr)
        {
            this.loc = loc;
        }
    }
}