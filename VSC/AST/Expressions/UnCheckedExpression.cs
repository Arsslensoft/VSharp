using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

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

        public override IConstantValue BuilConstantValue(ResolveContext rc, bool isAttributeConstant)
        {
            Constant v = Expr.BuilConstantValue(rc, isAttributeConstant) as Constant;
            if (v != null)
                return new ConstantCheckedExpression(false, v);
            else
                return null;
        }
    }
}