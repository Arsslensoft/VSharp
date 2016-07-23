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
        public override Expression DoResolve(ResolveContext rc)
        {
            var res = expr.DoResolve(rc);
            var constant = res as Constant;
            if (constant != null && constant.IsLiteral)
                return Constant.CreateConstantFromValue(rc,res.Type, constant.GetValue(), expr.Location);

            return res;
        }

        public override Expression DoResolveLeftValue(ResolveContext ec, Expression right_side)
        {
            return expr.DoResolveLeftValue(ec, right_side);
        }
        //public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        //{
        //    return Expr.BuilConstantValue(isAttributeConstant);
        //}
    }
}