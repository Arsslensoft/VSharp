namespace VSC.AST
{
    public abstract class ShimExpression : Expression
    {
        protected Expression expr;

        protected ShimExpression(Expression expr)
        {
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