using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class DefaultValueExpression : Expression, IConstantValue
    {
        Expression expr;

        public DefaultValueExpression(Expression expr, Location loc)
        {
            this.expr = expr;
            this.loc = loc;
        }

        public Expression Expr
        {
            get
            {
                return this.expr;
            }
        }

        public override Expression DoResolve(ResolveContext rc)
        {
            eclass = ExprClass.Variable;
            IType tpexpr = (expr as ITypeReference).Resolve(rc);
            if (tpexpr == null)
                return null;

            if (rc.IsStaticType(tpexpr))
                rc.Report.Error(-244, loc, "The `default value' operator cannot be applied to an operand of a static type");
            

            if (tpexpr is PointerTypeSpec)
                return new NullConstant(tpexpr, Location);

            if (tpexpr.IsReferenceType.HasValue && tpexpr.IsReferenceType.Value)
                return new NullConstant(tpexpr,loc);

            // TODO:Add cast to nullable/struct
            return Constant.CreateConstantFromValue(rc,tpexpr, ResolveContext.GetDefaultValue(tpexpr),loc);
        }

        public override Expression Constantify(ResolveContext resolver)
        {
            return DoResolve(resolver);
        }
    }
}