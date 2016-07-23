using System;
using System.Linq.Expressions;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
using ConstantExpression = VSC.TypeSystem.Resolver.ConstantExpression;

namespace VSC.AST
{
    public class UnaryExpression : Expression
    {
        public Expression Expr;
        public readonly VSC.TypeSystem.Resolver.UnaryOperatorType Oper;
        IMethod userDefinedOperatorMethod;
        bool isLiftedOperator;
        private System.Linq.Expressions.ExpressionType operatortype;


        public UnaryExpression(VSC.TypeSystem.Resolver.UnaryOperatorType op, Expression expr, Location loc)
        {
            Oper = op;
            Expr = expr;
            this.loc = loc;
        }
        public override Expression DoResolveLeftValue(ResolveContext ec, Expression right)
        {
            return null;
        }


        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            Expr = Expr.DoResolve(rc);
            if (Expr == null)
                return null;
            ITypeDefinition inputTypeDef = Expr.Type.GetDefinition();
            if (Expr.IsCompileTimeConstant && Expr is Constant && inputTypeDef != null)
            {
                // Special cases for int.MinValue and long.MinValue
                if (inputTypeDef.KnownTypeCode == KnownTypeCode.UInt32 && 2147483648.Equals(Expr.ConstantValue))
                    return Constant.CreateConstantFromValue(rc,rc.Compilation.FindType(KnownTypeCode.Int32), -2147483648,Expr.Location);
                else if (inputTypeDef.KnownTypeCode == KnownTypeCode.UInt64 && 9223372036854775808.Equals(Expr.ConstantValue))
                    return Constant.CreateConstantFromValue(rc, rc.Compilation.FindType(KnownTypeCode.Int64), -9223372036854775808, Expr.Location);

            }
            Expression rr = ResolveUnaryOperator(rc,Oper,Expr);
            UnaryExpression uorr = rr as UnaryExpression;
            if (uorr == null)
            {
                rc.Report.Error(0, loc, "The `{0}' operator cannot be applied to operand of type `{1}'",
                OperName(Oper), ResolvedType.ToString());
                return null;
            }
            rr.eclass = ExprClass.Value;
            _resolved = true;
               
            return rr;
        }
        #region ResolveUnaryOperator
        #region ResolveUnaryOperator method
        public Expression ResolveUnaryOperator(ResolveContext rc, UnaryOperatorType op, Expression expression)
        {
            // V# 4.0 spec: §7.3.3 Unary operator overload resolution
            string overloadableOperatorName = GetOverloadableOperatorName(op);
            if (overloadableOperatorName == null)
            {
                switch (op)
                {
                    case UnaryOperatorType.Dereference:
                        PointerTypeSpec p = expression.Type as PointerTypeSpec;
                        if (p != null)
                            return SetOperationInformations(rc,p.ElementType, op, expression);
                        else
                            return ErrorResult;
                    case UnaryOperatorType.AddressOf:
                        return SetOperationInformations(rc, new PointerTypeSpec(expression.Type), op, expression);


                    default:
                        return ErrorExpression.UnknownError;
                }
            }
            // If the type is nullable, get the underlying type:
            IType type = NullableType.GetUnderlyingType(expression.Type);
            bool isNullable = NullableType.IsNullable(expression.Type);

            // the operator is overloadable:
            OverloadResolution userDefinedOperatorOR = rc.CreateOverloadResolution(new[] { expression });
            foreach (var candidate in rc.GetUserDefinedOperatorCandidates(type, overloadableOperatorName))
            {
                userDefinedOperatorOR.AddCandidate(candidate);
            }
            if (userDefinedOperatorOR.FoundApplicableCandidate)
            {
                return SetUserDefinedOperationInformations(rc,userDefinedOperatorOR);
            }

            expression = UnaryNumericPromotion(rc, op, ref type, isNullable, expression);
            VSharpOperators.OperatorMethod[] methodGroup;
            VSharpOperators operators = VSharpOperators.Get(rc.compilation);
            switch (op)
            {
                case UnaryOperatorType.PreIncrement:
                case UnaryOperatorType.Decrement:
                case UnaryOperatorType.PostIncrement:
                case UnaryOperatorType.PostDecrement:
                    // V# 4.0 spec: §7.6.9 Postfix increment and decrement operators
                    // V# 4.0 spec: §7.7.5 Prefix increment and decrement operators
                    TypeCode code = ReflectionHelper.GetTypeCode(type);
                    if ((code >= TypeCode.Char && code <= TypeCode.Decimal) || type.Kind == TypeKind.Enum || type.Kind == TypeKind.Pointer)
                        return SetOperationInformations(rc, expression.Type, op, expression, isNullable);
                    else
                        return new ErrorExpression(expression.Type);
                case UnaryOperatorType.UnaryPlus:
                    methodGroup = operators.UnaryPlusOperators;
                    break;
                case UnaryOperatorType.UnaryNegation:
                    methodGroup = rc.checkForOverflow ? operators.CheckedUnaryMinusOperators : operators.UncheckedUnaryMinusOperators;
                    break;
                case UnaryOperatorType.LogicalNot:
                    methodGroup = operators.LogicalNegationOperators;
                    break;
                case UnaryOperatorType.OnesComplement:
                    if (type.Kind == TypeKind.Enum)
                    {
                        if (expression.IsCompileTimeConstant && !isNullable && expression.ConstantValue != null)
                        {
                            // evaluate as (E)(~(U)x);
                            var U = rc.compilation.FindType(expression.ConstantValue.GetType());
                            var unpackedEnum = new ConstantExpression(U, expression.ConstantValue);
                            var rr = ResolveUnaryOperator(rc, op, unpackedEnum);
                            ResolveContext ovfrc = rc.WithCheckForOverflow(false);
                            rr = new CastExpression(type, rr).DoResolve(ovfrc);
                            if (rr.IsCompileTimeConstant)
                                return rr;
                        }
                        return SetOperationInformations(rc, expression.Type, op, expression, isNullable);
                    }
                    else
                    {
                        methodGroup = operators.BitwiseComplementOperators;
                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }
            OverloadResolution builtinOperatorOR = rc.CreateOverloadResolution(new[] { expression });
            foreach (var candidate in methodGroup)
            {
                builtinOperatorOR.AddCandidate(candidate);
            }
            VSharpOperators.UnaryOperatorMethod m = (VSharpOperators.UnaryOperatorMethod)builtinOperatorOR.BestCandidate;
            IType resultType = m.ReturnType;
            if (builtinOperatorOR.BestCandidateErrors != OverloadResolutionErrors.None)
            {
                if (userDefinedOperatorOR.BestCandidate != null)
                {
                    // If there are any user-defined operators, prefer those over the built-in operators.
                    // It'll be a more informative error.
                    return SetUserDefinedOperationInformations(rc, userDefinedOperatorOR);
                }
                else if (builtinOperatorOR.BestCandidateAmbiguousWith != null)
                {
                    // If the best candidate is ambiguous, just use the input type instead
                    // of picking one of the ambiguous overloads.
                    return new ErrorExpression(expression.Type);
                }
                else
                {
                    return new ErrorExpression(resultType);
                }
            }
            else if (expression.IsCompileTimeConstant && m.CanEvaluateAtCompileTime)
            {
                object val;
                try
                {
                    val = m.Invoke(rc, expression.ConstantValue);
                }
                catch (ArithmeticException)
                {
                    return new ErrorExpression(resultType);
                }
                return new ConstantExpression(resultType, val);
            }
            else
            {
                expression = rc.Convert(expression, m.Parameters[0].Type, builtinOperatorOR.ArgumentConversions[0]);
                return SetOperationInformations(rc, resultType, op, expression,
                                                  builtinOperatorOR.BestCandidate is OverloadResolution.ILiftedOperator);
            }
        }
        Expression SetUserDefinedOperationInformations(ResolveContext rc, OverloadResolution r)
        {//TODO:Is It OK
            if (r.BestCandidateErrors != OverloadResolutionErrors.None)
            {
                rc.Report.Error(0, loc, "Operator `{0}' is ambiguous on an operand of type `{1}'",
                OperName(Oper), Expr.Type.ToString());
                return r.CreateInvocation(null);
            }


            IMethod method = (IMethod)r.BestCandidate;
            var operands = r.GetArgumentsWithConversions();
            this.operatortype = ResolveContext.GetLinqNodeType(this.Oper, rc.checkForOverflow);
            this.isLiftedOperator = method is OverloadResolution.ILiftedOperator;
            this.ResolvedType = method.ReturnType;
            this.Expr = operands[0];
            this.userDefinedOperatorMethod = method;
            _resolved = true;
            return this;
        }
        
        Expression SetOperationInformations(ResolveContext rc,IType resultType, UnaryOperatorType op, Expression expression, bool isLifted = false)
        {
            return new OperatorExpression(
                resultType,ResolveContext.GetLinqNodeType(op, rc.checkForOverflow),
                null, isLifted, new[] { expression });
        }
        #endregion

        #region UnaryNumericPromotion
        IType MakeNullable(ResolveContext rc, IType type, bool isNullable)
        {
            if (isNullable)
                return NullableType.Create(rc.compilation, type);
            else
                return type;
        }
        Expression UnaryNumericPromotion(ResolveContext rc, UnaryOperatorType op, ref IType type, bool isNullable, Expression expression)
        {
            // V# 4.0 spec: §7.3.6.1
            TypeCode code = ReflectionHelper.GetTypeCode(type);
            if (isNullable && type.Kind == TypeKind.Null)
                code = TypeCode.SByte; // cause promotion of null to int32
            switch (op)
            {
                case UnaryOperatorType.UnaryNegation:
                    if (code == TypeCode.UInt32)
                    {
                        type = rc.compilation.FindType(KnownTypeCode.Int64);
                        return rc.Convert(expression, MakeNullable(rc, type, isNullable),
                                       isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
                    }
                    goto case UnaryOperatorType.UnaryPlus;
                case UnaryOperatorType.UnaryPlus:
                case UnaryOperatorType.OnesComplement:
                    if (code >= TypeCode.Char && code <= TypeCode.UInt16)
                    {
                        type = rc.compilation.FindType(KnownTypeCode.Int32);
                        return rc.Convert(expression, MakeNullable(rc, type, isNullable),
                                       isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
                    }
                    break;
            }
            return expression;
        }
        #endregion

        #region GetOverloadableOperatorName
        static string GetOverloadableOperatorName(UnaryOperatorType op)
        {
            switch (op)
            {
                case UnaryOperatorType.LogicalNot:
                    return "op_LogicalNot";
                case UnaryOperatorType.OnesComplement:
                    return "op_OnesComplement";
                case UnaryOperatorType.UnaryNegation:
                    return "op_UnaryNegation";
                case UnaryOperatorType.UnaryPlus:
                    return "op_UnaryPlus";
                case UnaryOperatorType.PreIncrement:
                case UnaryOperatorType.PostIncrement:
                    return "op_Increment";
                case UnaryOperatorType.Decrement:
                case UnaryOperatorType.PostDecrement:
                    return "op_Decrement";
                //case UnaryOperatorType.UnaryOperator:
                //    return "op_U_";
                default:
                    return null;
            }
        }
        #endregion
        #endregion

        public static string OperName(UnaryOperatorType oper)
        {
            switch (oper)
            {
                case UnaryOperatorType.UnaryPlus:
                    return "+";
                case UnaryOperatorType.UnaryNegation:
                    return "-";
                case UnaryOperatorType.LogicalNot:
                    return "!";
                case UnaryOperatorType.OnesComplement:
                    return "~";
                case UnaryOperatorType.AddressOf:
                    return "&";
            }

            throw new NotImplementedException(oper.ToString());
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