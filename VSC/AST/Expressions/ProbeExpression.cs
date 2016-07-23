using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public abstract class ProbeExpression : Expression
    {
        public Expression ProbeType;
        protected Expression expr;
        protected IType probe_type_expr;
        protected ProbeExpression(Expression expr, Expression probe_type, Location l)
        {
            ProbeType = probe_type;
            loc = l;
            this.expr = expr;
        }

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }
        protected abstract string OperatorName { get; }
        protected virtual void ResolveProbeType(ResolveContext rc)
        {
            probe_type_expr = ProbeType.ResolveAsType(rc);
        }
        protected Expression ResolveCommon(ResolveContext rc)
        {
            expr = expr.DoResolve(rc);
            if (expr == null)
                return null;

            ResolveProbeType(rc);
            if (probe_type_expr == null)
                return this;

            if (rc.IsStaticType(probe_type_expr))
            {
                rc.Report.Error(250, loc, "The second operand of `is' or `as' operator cannot be static type `{0}'",
                    probe_type_expr.ToString());
                return null;
            }

            if (expr.Type is PointerTypeSpec || probe_type_expr is PointerTypeSpec)
            {
                rc.Report.Error(251, loc, "The `{0}' operator cannot be applied to an operand of pointer type",
                    OperatorName);
                return null;
            }
            //TODO:Method Group type & anonymous method type
            if (/*expr.Type == InternalType.AnonymousMethod ||*/ expr is MethodGroupExpression)
            {
                rc.Report.Error(837, loc, "The `{0}' operator cannot be applied to a lambda expression, anonymous method, or method group",
                    OperatorName);
                return null;
            }

            return this;
        }
    }
}