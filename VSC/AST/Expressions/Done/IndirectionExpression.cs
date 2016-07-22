using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class IndirectionExpression : Expression
    {
        Expression expr;
        public IndirectionExpression(Expression expr, Location l)
        {
            this.expr = expr;
            loc = l;
        }

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }

        public override Expression DoResolveLeftValue(ResolveContext ec, Expression right_side)
        {
            return DoResolve(ec);
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            expr = expr.DoResolve(rc);
            if (expr == null)
                return null;

            var pc = expr.Type as PointerTypeSpec;

            if (pc == null)
            {
                rc.Report.Error(0, loc, "The * or -> operator must be applied to a pointer");
                return null;
            }

            ResolvedType = pc.ElementType;

            if (ResolvedType.Kind == TypeKind.Void)
            {
                rc.Report.Error(0, loc, "The operation in question is undefined on void pointers");
                return null;
            }

            eclass = ExprClass.Variable;
            return this;
        }

    }
}