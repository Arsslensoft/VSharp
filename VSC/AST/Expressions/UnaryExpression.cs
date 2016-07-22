using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

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

        //public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        //{
        //    Constant v = Expr.BuilConstantValue(isAttributeConstant) as Constant;
        //    if (v == null)
        //        return null;
        //    switch (Oper)
        //    {
        //        case UnaryOperatorType.LogicalNot:
        //        case UnaryOperatorType.OnesComplement:
        //        case UnaryOperatorType.UnaryNegation:
        //        case UnaryOperatorType.UnaryPlus:
        //            return new ConstantUnaryOperator(Oper, v);
        //        default:
        //            return null;
        //    }
        //}
    }
}