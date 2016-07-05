namespace VSC.AST
{
    public class ContextualReturn : Return
    {
        ExpressionStatement statement;

        public ContextualReturn(Expression expr)
            : base(expr, expr.Location)
        {
        }
    }
}