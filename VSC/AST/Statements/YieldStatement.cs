namespace VSC.AST
{
    public abstract class YieldStatement : ResumableStatement
    {
        protected Expression expr;

        protected YieldStatement(Expression expr, Location l)
        {
            this.expr = expr;
            loc = l;
        }

        public Expression Expr
        {
            get { return this.expr; }
        }
    }
}