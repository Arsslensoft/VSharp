namespace VSC.AST
{
    public class DefaultValueExpression : Expression
    {
        Expression expr;

        public DefaultValueExpression(Expression expr, Location loc)
        {
            this.expr = expr;
            this.loc = loc;
        }

        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }

    }
}