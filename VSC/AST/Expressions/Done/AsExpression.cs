using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    ///   Implementation of the `as' operator.
    /// </summary>
    public class AsExpression : ProbeExpression
    {

        public AsExpression(Expression expr, Expression probe_type, Location l)
            : base(expr, probe_type, l)
        {
        }
        protected override string OperatorName
        {
            get { return "as"; }
        }
        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            if (ResolveCommon(rc) == null)
                return null;

            ResolvedType = probe_type_expr;
            eclass = ExprClass.Value;
            IType etype = expr.Type;

            if (ResolvedType.IsReferenceType.HasValue && !ResolvedType.IsReferenceType.Value && !NullableType.IsNullable(ResolvedType))
            {
                if (ResolvedType is TypeParameterSpec)
                    rc.Report.Error(413, loc,
                        "The `as' operator cannot be used with a non-reference type parameter `{0}'. Consider adding `class' or a reference type constraint",
                        probe_type_expr.ToString());
                else
                    rc.Report.Error(77, loc,
                        "The `as' operator cannot be used with a non-nullable value type `{0}'",
                        ResolvedType.ToString());
            
                return null;
            }

           
            // If the compile-time type of E is dynamic, unlike the cast operator the as operator is not dynamically bound
            if (etype.Kind == TypeKind.Dynamic)
                return this;

            return new CastExpression(probe_type_expr, expr, Conversion.TryCast, rc.checkForOverflow,loc);
        }

    }
}