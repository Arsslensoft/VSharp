namespace VSC.AST
{
    public abstract class ProbeExpression : Expression
    {
        public Expression ProbeType;
        protected Expression expr;

        protected ProbeExpression(Expression expr, Expression probe_type, Location l)
        {
            ProbeType = probe_type;
            loc = l;
            this.expr = expr;
        }

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }
    }
}