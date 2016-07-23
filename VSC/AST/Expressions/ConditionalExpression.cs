using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Implements the ternary conditional operator (?:)
    /// </summary>
    public class ConditionalExpression : Expression
    {
       internal Expression expr, true_expr, false_expr;

        public ConditionalExpression(Expression expr, Expression true_expr, Expression false_expr, Location loc)
        {
            this.expr = expr;
            this.true_expr = true_expr;
            this.false_expr = false_expr;
            this.loc = loc;
        }

        #region Properties

        public Expression Expr
        {
            get
            {
                return expr;
            }
        }

        public Expression TrueExpr
        {
            get
            {
                return true_expr;
            }
        }

        public Expression FalseExpr
        {
            get
            {
                return false_expr;
            }
        }

        #endregion

        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            expr = expr.DoResolve(rc);
            true_expr = true_expr.DoResolve(rc);
            false_expr = false_expr.DoResolve(rc);

            if (true_expr == null || false_expr == null || expr == null)
                return null;

            eclass = ExprClass.Value;


            // V# 4.0 spec §7.14: Conditional operator

            bool isValid;
            IType resultType;
            if (true_expr.Type.Kind == TypeKind.Dynamic || false_expr.Type.Kind == TypeKind.Dynamic)
            {
                resultType = SpecialTypeSpec.Dynamic;
                isValid = rc.TryConvert(ref true_expr, resultType) & rc.TryConvert(ref false_expr, resultType);
            }
            else if (rc.HasType(true_expr) && rc.HasType(false_expr))
            {
                Conversion t2f = rc.conversions.ImplicitConversion(true_expr, false_expr.Type);
                Conversion f2t = rc.conversions.ImplicitConversion(false_expr, true_expr.Type);
                // The operator is valid:
                // a) if there's a conversion in one direction but not the other
                // b) if there are conversions in both directions, and the types are equivalent
                if (rc.IsBetterConditionalConversion(t2f, f2t))
                {
                    resultType = false_expr.Type;
                    isValid = true;
                    true_expr = rc.Convert(true_expr, resultType, t2f);
                }
                else if (rc.IsBetterConditionalConversion(f2t, t2f))
                {
                    resultType = true_expr.Type;
                    isValid = true;
                    false_expr = rc.Convert(false_expr, resultType, f2t);
                }
                else
                {
                    resultType = true_expr.Type;
                    isValid = true_expr.Type.Equals(false_expr.Type);
                    if (!isValid)
                    {
                        _resolved = true;
                        ResolvedType = resultType;
                        rc.Report.Error(248, true_expr.Location,
                              "Type of conditional expression cannot be determined as `{0}' and `{1}' convert implicitly to each other",
                                          true_expr.Type.ToString(), false_expr.Type.ToString());
                        return this;
                    }
                }
            }
            else if (rc.HasType(true_expr))
            {
                resultType = true_expr.Type;
                isValid = rc.TryConvert(ref false_expr, resultType);
            }
            else if (rc.HasType(false_expr))
            {
                resultType = false_expr.Type;
                isValid = rc.TryConvert(ref true_expr, resultType);
            }
            else
            {
                return ErrorResult;
            }
            expr = rc.ResolveCondition(expr);
            if (isValid)
            {
                if (expr.IsCompileTimeConstant && true_expr.IsCompileTimeConstant && false_expr.IsCompileTimeConstant)
                {
                    bool? val = expr.ConstantValue as bool?;
                    if (val == true)
                        return true_expr;
                    else if (val == false)
                        return false_expr;
                }
                _resolved = true;
                this.ResolvedType = resultType;
                return this;
            }
            else
                rc.Report.Error(249, true_expr.Location,
                                  "Type of conditional expression cannot be determined because there is no implicit conversion between `{0}' and `{1}'",
                                  true_expr.Type.ToString(), false_expr.Type.ToString());
           
            return this;
        }
    }
}