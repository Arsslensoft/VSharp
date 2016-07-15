using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class ParenthesizedExpression : ShimExpression
    {
        public ParenthesizedExpression(Expression expr, Location loc)
            : base(expr)
        {
            this.loc = loc;
        }

        public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        {
            return Expr.BuilConstantValue(isAttributeConstant);
        }
    }
}