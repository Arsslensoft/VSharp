namespace VSC.AST
{
    /// <summary>
    ///   Implements checked expressions
    /// </summary>
    public class CheckedExpression : Expression
    {

        public Expression Expr;

        public CheckedExpression(Expression e, Location l)
        {
            Expr = e;
            loc = l;
        }

    }
}