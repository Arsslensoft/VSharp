namespace VSC.AST
{
    public class IndirectionExpression : Expression
    {
        Expression expr;
        public IndirectionExpression(Expression expr, Location l)
        {
            this.expr = expr;
            loc = l;
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