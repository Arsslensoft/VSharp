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
        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            if (_resolved)
                return this;

            ResolveContext oldResolver = rc;
            try
            {
                rc = rc.WithCheckForOverflow(false);
                Expr = Expr.DoResolve(rc);

            }
            finally
            {
                rc = oldResolver;
            }


            if (Expr == null)
                return null;

            //TODO:Anonymous
            if (Expr is Constant || Expr is MethodGroupExpression ||  /*Expr is AnonymousMethodExpression ||*/ Expr is DefaultValueExpression)
                return Expr;

            ResolvedType = Expr.Type;
            _resolved = true;
            eclass = Expr.eclass;
            return this;
        }
        //public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        //{
        //    Constant v = Expr.BuilConstantValue(isAttributeConstant) as Constant;
        //    if (v != null)
        //        return new ConstantCheckedExpression(false, v);
        //    else
        //        return null;
        //}
    }
}