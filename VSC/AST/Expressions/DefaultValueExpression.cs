using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

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

        public override IConstantValue BuilConstantValue(ResolveContext rc, bool isAttributeConstant)
        {
            return new ConstantDefaultValue(Expr as ITypeReference);
        }
    }
}