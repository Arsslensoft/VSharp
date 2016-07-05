namespace VSC.AST
{
    public class UnaryExpression : Expression
    {
        public Expression Expr;
        public readonly VSC.TypeSystem.Resolver.UnaryOperatorType Oper;

        public UnaryExpression(VSC.TypeSystem.Resolver.UnaryOperatorType op, Expression expr, Location loc)
        {
            Oper = op;
            Expr = expr;
            this.loc = loc;
        }
    }
}