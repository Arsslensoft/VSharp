namespace VSC.AST
{
    public class CastExpression : ShimExpression
    {
        Expression target_type;

        public CastExpression(Expression cast_type, Expression expr, Location loc)
            : base(expr)
        {
            this.target_type = cast_type;
            this.loc = loc;
        }

        public Expression TargetType
        {
            get { return target_type; }
        }
    }
}