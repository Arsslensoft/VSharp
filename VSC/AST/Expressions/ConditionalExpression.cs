namespace VSC.AST
{
    /// <summary>
    ///   Implements the ternary conditional operator (?:)
    /// </summary>
    public class ConditionalExpression : Expression
    {
        Expression expr, true_expr, false_expr;

        public ConditionalExpression(Expression expr, Expression true_expr, Expression false_expr, Location loc)
        {
            this.expr = expr;
            this.true_expr = true_expr;
            this.false_expr = false_expr;
            this.loc = loc;
        }

        #region Properties

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }

        public Expression TrueExpr
        {
            get
            {
                return true_expr;
            }
        }

        public Expression FalseExpr
        {
            get
            {
                return false_expr;
            }
        }

        #endregion
    }
}