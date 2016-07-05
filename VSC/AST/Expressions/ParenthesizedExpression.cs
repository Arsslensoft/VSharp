namespace VSC.AST
{
    public class ParenthesizedExpression : ShimExpression
    {
        public ParenthesizedExpression(Expression expr, Location loc)
            : base(expr)
        {
            this.loc = loc;
        }
    }
}