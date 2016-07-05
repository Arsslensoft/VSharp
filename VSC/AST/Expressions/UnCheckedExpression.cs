namespace VSC.AST
{
    /// <summary>
    ///   Implements the unchecked expression
    /// </summary>
    public class UnCheckedExpression : Expression
    {

        public Expression Expr;

        public UnCheckedExpression(Expression e, Location l)
        {
            Expr = e;
            loc = l;
        }
    }
}