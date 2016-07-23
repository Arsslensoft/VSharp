using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implementation of the `is' operator.
    /// </summary>
    public class IsExpression : ProbeExpression
    {

        public IsExpression(Expression expr, Expression probe_type, Location l)
            : base(expr, probe_type, l)
        {
        }

        protected override string OperatorName
        {
            get { return "is"; }
        }

        Expression CreateConstantResult(ResolveContext rc, bool result)
        {
            if (result)
                rc.Report.Warning(252, 1, loc, "The given expression is always of the provided (`{0}') type",
                    probe_type_expr.ToString());
            else
                rc.Report.Warning(253, 1, loc, "The given expression is never of the provided (`{0}') type",
                    probe_type_expr.ToString());

            var c = new BoolConstant(result, loc);
            return c.DoResolve(rc);
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            if (ResolveCommon(rc) == null)
                return null;

            eclass = ExprClass.Value;
            ResolvedType = KnownTypeReference.Boolean.Resolve(rc);
          
            var res = ResolveResultExpression(rc);
            //if (Variable != null)
            //{
            //    if (res is Constant)
            //        throw new NotImplementedException("constant in type pattern matching");

            //    Variable.Type = probe_type_expr;
            //    var bc = rc as BlockContext;
            //    if (bc != null)
            //        Variable.PrepareAssignmentAnalysis(bc);
            //}

            return res;
        }


        Expression ResolveResultExpression(ResolveContext ec)
        {
            IType d = expr.Type;
            bool d_is_nullable = false;

            //
            // If E is a method group or the null literal, or if the type of E is a reference
            // type or a nullable type and the value of E is null, the result is false
            //
            if (expr is NullConstant || expr.eclass == ExprClass.MethodGroup)
                return CreateConstantResult(ec, false);

            if (NullableType.IsNullable(d))
            {

                var ut = NullableType.GetUnderlyingType(d);
                if (!(ut is TypeParameterSpec))
                {
                    d = ut;
                    d_is_nullable = true;
                }
            }

            IType t = probe_type_expr;
            bool t_is_nullable = false;
            if (NullableType.IsNullable(t))
            {
                var ut = NullableType.GetUnderlyingType(t);
                if (!(ut is TypeParameterSpec))
                {
                    t = ut;
                    t_is_nullable = true;
                }
            }

            //if (t.Kind == TypeKind.Struct)
            //{
            //    if (d == t)
            //    {
                  
            //        //
            //        // D and T are the same value types but D can be null
            //        //
            //        if (d_is_nullable && !t_is_nullable)
            //        {
            //            expr_unwrap = Nullable.Unwrap.Create(expr, true);
            //            return this;
            //        }

            //        //
            //        // The result is true if D and T are the same value types
            //        //
            //        return CreateConstantResult(ec, true);
            //    }

            //    var tp = d as TypeParameterSpec;
            //    if (tp != null)
            //        return ResolveGenericParameter(ec, t, tp);

            //    //
            //    // An unboxing conversion exists
            //    //
            //    if (Convert.ExplicitReferenceConversionExists(d, t))
            //        return this;

            //    //
            //    // open generic type
            //    //
            //    if (d is InflatedTypeSpec && InflatedTypeSpec.ContainsTypeParameter(d))
            //        return this;
            //}
            //else
            //{
            //    var tps = t as TypeParameterSpec;
            //    if (tps != null)
            //        return ResolveGenericParameter(ec, d, tps);

            //    if (t.BuiltinType == BuiltinTypeSpec.Type.Dynamic)
            //    {
            //        ec.Report.Warning(1981, 3, loc,
            //            "Using `{0}' to test compatibility with `{1}' is identical to testing compatibility with `object'",
            //            OperatorName, t.GetSignatureForError());
            //    }

            //    if (TypeManager.IsGenericParameter(d))
            //        return ResolveGenericParameter(ec, t, (TypeParameterSpec)d);

            //    if (TypeSpec.IsValueType(d))
            //    {
            //        if (Convert.ImplicitBoxingConversion(null, d, t) != null)
            //        {
            //            if (d_is_nullable && !t_is_nullable)
            //            {
            //                expr_unwrap = Nullable.Unwrap.Create(expr, false);
            //                return this;
            //            }

            //            return CreateConstantResult(ec, true);
            //        }
            //    }
            //    else
            //    {
            //        if (Convert.ImplicitReferenceConversionExists(d, t))
            //        {
            //            var c = expr as Constant;
            //            if (c != null)
            //                return CreateConstantResult(ec, !c.IsNull);

            //            //
            //            // Do not optimize for imported type or dynamic type
            //            //
            //            if (d.MemberDefinition.IsImported && d.BuiltinType != BuiltinTypeSpec.Type.None &&
            //                d.MemberDefinition.DeclaringAssembly != t.MemberDefinition.DeclaringAssembly)
            //            {
            //                return this;
            //            }

            //            if (d.BuiltinType == BuiltinTypeSpec.Type.Dynamic)
            //                return this;

            //            //
            //            // Turn is check into simple null check for implicitly convertible reference types
            //            //
            //            return ReducedExpression.Create(
            //                new Binary(Binary.Operator.Inequality, expr, new NullLiteral(loc), Binary.State.UserOperatorsExcluded).Resolve(ec),
            //                this).Resolve(ec);
            //        }

            //        if (Convert.ExplicitReferenceConversionExists(d, t))
            //            return this;

            //        //
            //        // open generic type
            //        //
            //        if ((d is InflatedTypeSpec || d.IsArray) && InflatedTypeSpec.ContainsTypeParameter(d))
            //            return this;
            //    }
            //}
            
            //TODO:Full Support IS EXPRESSION
            return CreateConstantResult(ec, false);
        }
    }
}