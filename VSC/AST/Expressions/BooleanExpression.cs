namespace VSC.AST
{
    public class BooleanExpression : ShimExpression
    {
        public BooleanExpression(Expression expr)
            : base(expr)
        {
            this.loc = expr.Location;
        }

    }
}