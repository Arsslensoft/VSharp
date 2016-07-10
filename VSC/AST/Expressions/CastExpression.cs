using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class CastExpression : ShimExpression
    {
        Expression target_type;

        public CastExpression(Expression cast_type, Expression expr, Location loc)
            : base(expr)
        {
            this.target_type = cast_type;
            this.loc = loc;
        }

        public Expression TargetType
        {
            get { return target_type; }
        }

        public override IConstantValue BuilConstantValue(ResolveContext rc, bool isAttributeConstant)
        {
            Constant v = Expr.BuilConstantValue(rc, isAttributeConstant) as Constant;
            if (v == null)
                return null;
            var typeReference = TargetType as ITypeReference;
            return new ConstantCast(typeReference, v, false);
        }
    }
}