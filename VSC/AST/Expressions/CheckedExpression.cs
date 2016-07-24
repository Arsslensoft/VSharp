using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements checked expressions
    /// </summary>
    public class CheckedExpression : Expression, IConstantValue
    {

        public Expression Expr;

        public CheckedExpression(Expression e, Location l)
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
                    rc = rc.WithCheckForOverflow(true);
                    Expr =  Expr.DoResolve(rc);

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
            eclass = Expr.eclass;
            ResolvedType = Expr.Type;
            _resolved = true;
            return this;
        }

        public override Expression Constantify(ResolveContext resolver)
        {
            return DoResolve(resolver);
        }
    }
}