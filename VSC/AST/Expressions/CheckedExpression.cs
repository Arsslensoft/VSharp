using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

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


        public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        {
            Constant v = Expr.BuilConstantValue(isAttributeConstant) as Constant;
            if (v != null)
                return new ConstantCheckedExpression(true, v);
            else
                return null;
        }
    }
}