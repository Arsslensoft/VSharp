using System;
using System.Collections.Generic;
using System.Diagnostics;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    ///   Binary operators
    /// </summary>
    public class BinaryExpression : Expression
    {
        readonly VSC.TypeSystem.Resolver.BinaryOperatorType oper;
        Expression left, right;
        IMethod userDefinedOperatorMethod;
        bool isLiftedOperator;
        private System.Linq.Expressions.ExpressionType operatortype;
        public bool IsCompound = false;
        public BinaryExpression(VSC.TypeSystem.Resolver.BinaryOperatorType oper, Expression left, Expression right)
            : this(oper, left, right, left.Location)
        {
        }

        public BinaryExpression(VSC.TypeSystem.Resolver.BinaryOperatorType oper, Expression left, Expression right, Location loc)
        {
            this.oper = oper;
            this.left = left;
            this.right = right;
            this.loc = loc;
        }

        #region Properties

        public VSC.TypeSystem.Resolver.BinaryOperatorType Oper
        {
            get
            {
                return oper;
            }
        }

        public Expression Left
        {
            get
            {
                return this.left;
            }
        }

        public Expression Right
        {
            get
            {
                return this.right;
            }
        }



        #endregion


        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;
            left = left.DoResolve(rc);
            right = right.DoResolve(rc);
            if (left == null || right == null)
                return null;
            Expression expr = ResolveBinaryOperator(rc, oper, left, right);
            if (expr == null)
            {
                rc.Report.Error(0, loc, "Operator `{0}' cannot be applied to operands of type `{1}' and `{2}'",
   OperName(oper), left.Type.ToString(), right.Type.ToString());
                return null;
            }
            expr.eclass = ExprClass.Value;
            return expr;
        }

        #region ResolveBinaryOperator
        #region ResolveBinaryOperator method
        public Expression ResolveBinaryOperator(ResolveContext rc, BinaryOperatorType op, Expression lhs, Expression rhs)
        {
            // V# 4.0 spec: §7.3.4 Binary operator overload resolution
            string overloadableOperatorName = ResolveContext.GetOverloadableOperatorName(op);
            if (overloadableOperatorName == null)
            {

                // Handle logical and/or exactly as bitwise and/or:
                // - If the user overloads a bitwise operator, that implicitly creates the corresponding logical operator.
                // - If both inputs are compile-time constants, it doesn't matter that we don't short-circuit.
                // - If inputs aren't compile-time constants, we don't evaluate anything, so again it doesn't matter that we don't short-circuit
                if (op == BinaryOperatorType.LogicalAnd)
                {
                    overloadableOperatorName = ResolveContext.GetOverloadableOperatorName(BinaryOperatorType.BitwiseAnd);
                }
                else if (op == BinaryOperatorType.LogicalOr)
                {
                    overloadableOperatorName = ResolveContext.GetOverloadableOperatorName(BinaryOperatorType.BitwiseOr);
                }
                else if (op == BinaryOperatorType.NullCoalescing)
                {
                    // null coalescing operator is not overloadable and needs to be handled separately
                    return ResolveNullCoalescingOperator(rc,lhs, rhs);
                }
                else
                {
                    return ErrorExpression.UnknownError;
                }
            }

            // If the type is nullable, get the underlying type:
            bool isNullable = NullableType.IsNullable(lhs.Type) || NullableType.IsNullable(rhs.Type);
            IType lhsType = NullableType.GetUnderlyingType(lhs.Type);
            IType rhsType = NullableType.GetUnderlyingType(rhs.Type);

            // the operator is overloadable:
            OverloadResolution userDefinedOperatorOR = rc.CreateOverloadResolution(new[] { lhs, rhs });
            HashSet<IParameterizedMember> userOperatorCandidates = new HashSet<IParameterizedMember>();
            userOperatorCandidates.UnionWith(rc.GetUserDefinedOperatorCandidates(lhsType, overloadableOperatorName));
            userOperatorCandidates.UnionWith(rc.GetUserDefinedOperatorCandidates(rhsType, overloadableOperatorName));
            foreach (var candidate in userOperatorCandidates)
            {
                userDefinedOperatorOR.AddCandidate(candidate);
            }
            if (userDefinedOperatorOR.FoundApplicableCandidate)
            {
                return SetUserDefinedOperationInformations(rc,userDefinedOperatorOR);
            }

            if (lhsType.Kind == TypeKind.Null && rhsType.IsReferenceType == false
                || lhsType.IsReferenceType == false && rhsType.Kind == TypeKind.Null)
            {
                isNullable = true;
            }
            if (op == BinaryOperatorType.LeftShift || op == BinaryOperatorType.RightShift)
            {
                // special case: the shift operators allow "var x = null << null", producing int?.
                if (lhsType.Kind == TypeKind.Null && rhsType.Kind == TypeKind.Null)
                    isNullable = true;
                // for shift operators, do unary promotion independently on both arguments
                lhs = UnaryNumericPromotion(rc, UnaryOperatorType.UnaryPlus, ref lhsType, isNullable, lhs);
                rhs = UnaryNumericPromotion(rc, UnaryOperatorType.UnaryPlus, ref rhsType, isNullable, rhs);
            }
            else
            {
                bool allowNullableConstants = op == BinaryOperatorType.Equality || op == BinaryOperatorType.Inequality;
                if (!BinaryNumericPromotion(rc,isNullable, ref lhs, ref rhs, allowNullableConstants))
                    return new ErrorExpression(lhs.Type);
            }
            // re-read underlying types after numeric promotion
            lhsType = NullableType.GetUnderlyingType(lhs.Type);
            rhsType = NullableType.GetUnderlyingType(rhs.Type);

            IEnumerable<VSharpOperators.OperatorMethod> methodGroup;
            VSharpOperators operators = VSharpOperators.Get(rc.compilation);
            switch (op)
            {
                case BinaryOperatorType.Multiply:
                    methodGroup = operators.MultiplicationOperators;
                    break;
                case BinaryOperatorType.Division:
                    methodGroup = operators.DivisionOperators;
                    break;
                case BinaryOperatorType.Modulus:
                    methodGroup = operators.RemainderOperators;
                    break;
                case BinaryOperatorType.Addition:
                    methodGroup = operators.AdditionOperators;
                    {
                        if (lhsType.Kind == TypeKind.Enum)
                        {
                            // E operator +(E x, U y);
                            IType underlyingType = MakeNullable(rc, ResolveContext.GetEnumUnderlyingType(lhsType), isNullable);
                            if (rc.TryConvertEnum(ref rhs, underlyingType, ref isNullable, ref lhs))
                            {
                                return HandleEnumOperator(rc,isNullable, lhsType, op, lhs, rhs);
                            }
                        }
                        if (rhsType.Kind == TypeKind.Enum)
                        {
                            // E operator +(U x, E y);
                            IType underlyingType = MakeNullable(rc, ResolveContext.GetEnumUnderlyingType(rhsType), isNullable);
                            if (rc.TryConvertEnum(ref lhs, underlyingType, ref isNullable, ref rhs))
                            {
                                return HandleEnumOperator(rc,isNullable, rhsType, op, lhs, rhs);
                            }
                        }

                        if (lhsType.Kind == TypeKind.Delegate && rc.TryConvert(ref rhs, lhsType))
                        {
                            return SetOperationInformations(rc, lhsType, lhs, op, rhs);
                        }
                        else if (rhsType.Kind == TypeKind.Delegate && rc.TryConvert(ref lhs, rhsType))
                        {
                            return SetOperationInformations(rc, rhsType, lhs, op, rhs);
                        }

                        if (lhsType is PointerTypeSpec)
                        {
                            methodGroup = new[] {
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.Int32),
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.UInt32),
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.Int64),
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.UInt64)
							};
                        }
                        else if (rhsType is PointerTypeSpec)
                        {
                            methodGroup = new[] {
								PointerArithmeticOperator(rc,rhsType, KnownTypeCode.Int32, rhsType),
								PointerArithmeticOperator(rc,rhsType, KnownTypeCode.UInt32, rhsType),
								PointerArithmeticOperator(rc,rhsType, KnownTypeCode.Int64, rhsType),
								PointerArithmeticOperator(rc,rhsType, KnownTypeCode.UInt64, rhsType)
							};
                        }
                        if (lhsType.Kind == TypeKind.Null && rhsType.Kind == TypeKind.Null)
                            return new ErrorExpression(SpecialTypeSpec.NullType);
                    }
                    break;
                case BinaryOperatorType.Subtraction:
                    methodGroup = operators.SubtractionOperators;
                    {
                        if (lhsType.Kind == TypeKind.Enum)
                        {
                            // U operator –(E x, E y);
                            if (rc.TryConvertEnum(ref rhs, lhs.Type, ref isNullable, ref lhs, allowConversionFromConstantZero: false))
                            {
                                return HandleEnumSubtraction(rc,isNullable, lhsType, lhs, rhs);
                            }

                            // E operator –(E x, U y);
                            IType underlyingType = MakeNullable(rc, ResolveContext.GetEnumUnderlyingType(lhsType), isNullable);
                            if (rc.TryConvertEnum(ref rhs, underlyingType, ref isNullable, ref lhs))
                            {
                                return HandleEnumOperator(rc,isNullable, lhsType, op, lhs, rhs);
                            }
                        }
                        if (rhsType.Kind == TypeKind.Enum)
                        {
                            // U operator –(E x, E y);
                            if (rc.TryConvertEnum(ref lhs, rhs.Type, ref isNullable, ref rhs, allowConversionFromConstantZero: false))
                            {
                                return HandleEnumSubtraction(rc,isNullable, rhsType, lhs, rhs);
                            }

                            // E operator -(U x, E y);
                            IType underlyingType = MakeNullable(rc, ResolveContext.GetEnumUnderlyingType(rhsType), isNullable);
                            if (rc.TryConvertEnum(ref lhs, underlyingType, ref isNullable, ref rhs))
                            {
                                return HandleEnumOperator(rc,isNullable, rhsType, op, lhs, rhs);
                            }
                        }

                        if (lhsType.Kind == TypeKind.Delegate && rc.TryConvert(ref rhs, lhsType))
                        {
                            return SetOperationInformations(rc, lhsType, lhs, op, rhs);
                        }
                        else if (rhsType.Kind == TypeKind.Delegate && rc.TryConvert(ref lhs, rhsType))
                        {
                            return SetOperationInformations(rc, rhsType, lhs, op, rhs);
                        }

                        if (lhsType is PointerTypeSpec)
                        {
                            if (rhsType is PointerTypeSpec)
                            {
                                IType int64 = rc.compilation.FindType(KnownTypeCode.Int64);
                                if (lhsType.Equals(rhsType))
                                {
                                    return SetOperationInformations(rc, int64, lhs, op, rhs);
                                }
                                else
                                {
                                    return new ErrorExpression(int64);
                                }
                            }
                            methodGroup = new[] {
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.Int32),
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.UInt32),
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.Int64),
								PointerArithmeticOperator(rc,lhsType, lhsType, KnownTypeCode.UInt64)
							};
                        }

                        if (lhsType.Kind == TypeKind.Null && rhsType.Kind == TypeKind.Null)
                            return new ErrorExpression(SpecialTypeSpec.NullType);
                    }
                    break;
                case BinaryOperatorType.LeftShift:
                    methodGroup = operators.ShiftLeftOperators;
                    break;
                case BinaryOperatorType.RightShift:
                    methodGroup = operators.ShiftRightOperators;
                    break;
                case BinaryOperatorType.RotateRight:
                    methodGroup = operators.RotateRightOperators;
                    break;
                case BinaryOperatorType.RotateLeft:
                    methodGroup = operators.RotateLeftOperators;
                    break;


                case BinaryOperatorType.Equality:
                case BinaryOperatorType.Inequality:
                case BinaryOperatorType.LessThan:
                case BinaryOperatorType.GreaterThan:
                case BinaryOperatorType.LessThanOrEqual:
                case BinaryOperatorType.GreaterThanOrEqual:
                    {
                        if (lhsType.Kind == TypeKind.Enum && rc.TryConvert(ref rhs, lhs.Type))
                        {
                            // bool operator op(E x, E y);
                            return HandleEnumComparison(rc,op, lhsType, isNullable, lhs, rhs);
                        }
                        else if (rhsType.Kind == TypeKind.Enum && rc.TryConvert(ref lhs, rhs.Type))
                        {
                            // bool operator op(E x, E y);
                            return HandleEnumComparison(rc,op, rhsType, isNullable, lhs, rhs);
                        }
                        else if (lhsType is PointerTypeSpec && rhsType is PointerTypeSpec)
                        {
                            return SetOperationInformations(rc, rc.compilation.FindType(KnownTypeCode.Boolean), lhs, op, rhs);
                        }
                        if (op == BinaryOperatorType.Equality || op == BinaryOperatorType.Inequality)
                        {
                            if (lhsType.IsReferenceType == true && rhsType.IsReferenceType == true)
                            {
                                // If it's a reference comparison
                                if (op == BinaryOperatorType.Equality)
                                    methodGroup = operators.ReferenceEqualityOperators;
                                else
                                    methodGroup = operators.ReferenceInequalityOperators;
                                break;
                            }
                            else if (lhsType.Kind == TypeKind.Null && IsNullableTypeOrNonValueType(rhs.Type)
                                     || IsNullableTypeOrNonValueType(lhs.Type) && rhsType.Kind == TypeKind.Null)
                            {
                                // compare type parameter or nullable type with the null literal
                                return SetOperationInformations(rc, rc.compilation.FindType(KnownTypeCode.Boolean), lhs, op, rhs);
                            }
                        }
                        switch (op)
                        {
                            case BinaryOperatorType.Equality:
                                methodGroup = operators.ValueEqualityOperators;
                                break;
                            case BinaryOperatorType.Inequality:
                                methodGroup = operators.ValueInequalityOperators;
                                break;
                            case BinaryOperatorType.LessThan:
                                methodGroup = operators.LessThanOperators;
                                break;
                            case BinaryOperatorType.GreaterThan:
                                methodGroup = operators.GreaterThanOperators;
                                break;
                            case BinaryOperatorType.LessThanOrEqual:
                                methodGroup = operators.LessThanOrEqualOperators;
                                break;
                            case BinaryOperatorType.GreaterThanOrEqual:
                                methodGroup = operators.GreaterThanOrEqualOperators;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    break;
                case BinaryOperatorType.BitwiseAnd:
                case BinaryOperatorType.BitwiseOr:
                case BinaryOperatorType.ExclusiveOr:
                    {
                        if (lhsType.Kind == TypeKind.Enum)
                        {
                            // bool operator op(E x, E y);
                            if (rc.TryConvertEnum(ref rhs, lhs.Type, ref isNullable, ref lhs))
                            {
                                return HandleEnumOperator(rc,isNullable, lhsType, op, lhs, rhs);
                            }
                        }

                        if (rhsType.Kind == TypeKind.Enum)
                        {
                            // bool operator op(E x, E y);
                            if (rc.TryConvertEnum(ref lhs, rhs.Type, ref isNullable, ref rhs))
                            {
                                return HandleEnumOperator(rc,isNullable, rhsType, op, lhs, rhs);
                            }
                        }

                        switch (op)
                        {
                            case BinaryOperatorType.BitwiseAnd:
                                methodGroup = operators.BitwiseAndOperators;
                                break;
                            case BinaryOperatorType.BitwiseOr:
                                methodGroup = operators.BitwiseOrOperators;
                                break;
                            case BinaryOperatorType.ExclusiveOr:
                                methodGroup = operators.BitwiseXorOperators;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    break;
                case BinaryOperatorType.LogicalAnd:
                    methodGroup = operators.LogicalAndOperators;
                    break;
                case BinaryOperatorType.LogicalOr:
                    methodGroup = operators.LogicalOrOperators;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            OverloadResolution builtinOperatorOR = rc.CreateOverloadResolution(new[] { lhs, rhs });
            foreach (var candidate in methodGroup)
            {
                builtinOperatorOR.AddCandidate(candidate);
            }
            VSharpOperators.BinaryOperatorMethod m = (VSharpOperators.BinaryOperatorMethod)builtinOperatorOR.BestCandidate;
            IType resultType = m.ReturnType;
            if (builtinOperatorOR.BestCandidateErrors != OverloadResolutionErrors.None)
            {
                // If there are any user-defined operators, prefer those over the built-in operators.
                // It'll be a more informative error.
                if (userDefinedOperatorOR.BestCandidate != null)
                    return SetUserDefinedOperationInformations(rc, userDefinedOperatorOR);
                else
                    return new ErrorExpression(resultType);
            }
            else if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && m.CanEvaluateAtCompileTime)
            {
                object val;
                try
                {
                    val = m.Invoke(rc, lhs.ConstantValue, rhs.ConstantValue);
                }
                catch (ArithmeticException)
                {
                    return new ErrorExpression(resultType);
                }
                return new ConstantExpression(resultType, val);
            }
            else
            {
                lhs = rc.Convert(lhs, m.Parameters[0].Type, builtinOperatorOR.ArgumentConversions[0]);
                rhs = rc.Convert(rhs, m.Parameters[1].Type, builtinOperatorOR.ArgumentConversions[1]);
                return SetOperationInformations(rc, resultType, lhs, op, rhs,
                                                   builtinOperatorOR.BestCandidate is OverloadResolution.ILiftedOperator);
            }
        }
        Expression UnaryNumericPromotion(ResolveContext rc,UnaryOperatorType op, ref IType type, bool isNullable, Expression expression)
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
                        return rc.Convert(expression, MakeNullable(rc,type, isNullable),
                                       isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
                    }
                    goto case UnaryOperatorType.UnaryPlus;
                case UnaryOperatorType.UnaryPlus:
                case UnaryOperatorType.OnesComplement:
                    if (code >= TypeCode.Char && code <= TypeCode.UInt16)
                    {
                        type = rc.compilation.FindType(KnownTypeCode.Int32);
                        return rc.Convert(expression, MakeNullable(rc,type, isNullable),
                                       isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
                    }
                    break;
            }
            return expression;
        }
        bool IsNullableTypeOrNonValueType(IType type)
        {
            return NullableType.IsNullable(type) || type.IsReferenceType != false;
        }

        /// <summary>
        ///   Returns a stringified representation of the Operator
        /// </summary>
        string OperName(BinaryOperatorType oper)
        {
            string s;
            switch (oper)
            {
                case BinaryOperatorType.Multiply:
                    s = "*";
                    break;
                case BinaryOperatorType.Division:
                    s = "/";
                    break;
                case BinaryOperatorType.Modulus:
                    s = "%";
                    break;
                case BinaryOperatorType.Addition:
                    s = "+";
                    break;
                case BinaryOperatorType.Subtraction:
                    s = "-";
                    break;
                case BinaryOperatorType.LeftShift:
                    s = "<<";
                    break;
                case BinaryOperatorType.RightShift:
                    s = ">>";
                    break;
                case BinaryOperatorType.LessThan:
                    s = "<";
                    break;
                case BinaryOperatorType.GreaterThan:
                    s = ">";
                    break;
                case BinaryOperatorType.LessThanOrEqual:
                    s = "<=";
                    break;
                case BinaryOperatorType.GreaterThanOrEqual:
                    s = ">=";
                    break;
                case BinaryOperatorType.Equality:
                    s = "==";
                    break;
                case BinaryOperatorType.Inequality:
                    s = "!=";
                    break;
                case BinaryOperatorType.BitwiseAnd:
                    s = "&";
                    break;
                case BinaryOperatorType.BitwiseOr:
                    s = "|";
                    break;
                case BinaryOperatorType.ExclusiveOr:
                    s = "^";
                    break;
                case BinaryOperatorType.LogicalOr:
                    s = "||";
                    break;
                case BinaryOperatorType.LogicalAnd:
                    s = "&&";
                    break;
                default:
                    s = oper.ToString();
                    break;
            }

            if (IsCompound)
                return s + "=";

            return s;
        }
        Expression SetOperationInformations(ResolveContext rc,IType resultType, Expression lhs, BinaryOperatorType op, Expression rhs,bool isLifted = false)
        {
            this.isLiftedOperator = isLifted;
            this.ResolvedType = resultType;
            this.left = lhs;
            this.right = rhs;
            this.operatortype = ResolveContext.GetLinqNodeType(this.oper, rc.checkForOverflow);
            _resolved = true;
            return this;
        }
        Expression SetUserDefinedOperationInformations(ResolveContext rc,OverloadResolution r)
        {//TODO:Is It OK
            if (r.BestCandidateErrors != OverloadResolutionErrors.None)
            {
                rc.Report.Error(0, loc, "Operator `{0}' is ambiguous on operands of type `{1}' and `{2}'",
               OperName(oper), left.Type.ToString(), right.Type.ToString());
                return r.CreateInvocation(null);
            }
                

            IMethod method = (IMethod)r.BestCandidate;
            var operands = r.GetArgumentsWithConversions();
            this.operatortype = ResolveContext.GetLinqNodeType(this.oper, rc.checkForOverflow);
            this.isLiftedOperator = method is OverloadResolution.ILiftedOperator;
            this.ResolvedType = method.ReturnType;
            this.left = operands[0];
            this.right = operands[1];
            this.userDefinedOperatorMethod =method;
            _resolved = true;
            return this;
        }
        
        
        #endregion

        #region Pointer arithmetic
        VSharpOperators.BinaryOperatorMethod PointerArithmeticOperator(ResolveContext rc, IType resultType, IType inputType1, KnownTypeCode inputType2)
        {
            return PointerArithmeticOperator(rc,resultType, inputType1, rc.compilation.FindType(inputType2));
        }

        VSharpOperators.BinaryOperatorMethod PointerArithmeticOperator(ResolveContext rc, IType resultType, KnownTypeCode inputType1, IType inputType2)
        {
            return PointerArithmeticOperator(rc,resultType, rc.compilation.FindType(inputType1), inputType2);
        }

        VSharpOperators.BinaryOperatorMethod PointerArithmeticOperator(ResolveContext rc, IType resultType, IType inputType1, IType inputType2)
        {
            return new VSharpOperators.BinaryOperatorMethod(rc.compilation)
            {
                ReturnType = resultType,
                Parameters = {
					new ParameterSpec(inputType1, string.Empty),
					new ParameterSpec(inputType2, string.Empty)
				}
            };
        }
        #endregion

        #region Enum helper methods
        /// <summary>
        /// Handle the case where an enum value is compared with another enum value
        /// bool operator op(E x, E y);
        /// </summary>
        Expression HandleEnumComparison(ResolveContext rc, BinaryOperatorType op, IType enumType, bool isNullable, Expression lhs, Expression rhs)
        {
            // evaluate as ((U)x op (U)y)
            IType elementType = ResolveContext.GetEnumUnderlyingType(enumType);
            if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && !isNullable && elementType.Kind != TypeKind.Enum)
            {
                var rr = ResolveBinaryOperator(rc,op, new CastExpression(elementType, lhs).DoResolve(rc), new CastExpression(elementType, rhs).DoResolve(rc));
                if (rr.IsCompileTimeConstant)
                    return rr;
            }
            IType resultType = rc.compilation.FindType(KnownTypeCode.Boolean);
            return SetOperationInformations(rc, resultType, lhs, op, rhs, isNullable);
        }

        /// <summary>
        /// Handle the case where an enum value is subtracted from another enum value
        /// U operator –(E x, E y);
        /// </summary>
        Expression HandleEnumSubtraction(ResolveContext rc, bool isNullable, IType enumType, Expression lhs, Expression rhs)
        {
            // evaluate as (U)((U)x – (U)y)
            IType elementType = ResolveContext.GetEnumUnderlyingType(enumType);
            if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && !isNullable && elementType.Kind != TypeKind.Enum)
            {
                var rr = ResolveBinaryOperator(rc,BinaryOperatorType.Subtraction, new CastExpression(elementType, lhs).DoResolve(rc), new CastExpression(elementType, rhs).DoResolve(rc));
                rr = new CastExpression(elementType, rr).DoResolve(rc.WithCheckForOverflow(false));
                if (rr.IsCompileTimeConstant)
                    return rr;
            }
            IType resultType = MakeNullable(rc,elementType, isNullable);
            return SetOperationInformations(rc, resultType, lhs, BinaryOperatorType.Subtraction, rhs, isNullable);
        }

        /// <summary>
        /// Handle the following enum operators:
        /// E operator +(E x, U y);
        /// E operator +(U x, E y);
        /// E operator –(E x, U y);
        /// E operator &amp;(E x, E y);
        /// E operator |(E x, E y);
        /// E operator ^(E x, E y);
        /// </summary>
        Expression HandleEnumOperator(ResolveContext rc, bool isNullable, IType enumType, BinaryOperatorType op, Expression lhs, Expression rhs)
        {
            // evaluate as (E)((U)x op (U)y)
            if (lhs.IsCompileTimeConstant && rhs.IsCompileTimeConstant && !isNullable)
            {
                IType elementType = ResolveContext.GetEnumUnderlyingType(enumType);
                if (elementType.Kind != TypeKind.Enum)
                {
                    var rr = ResolveBinaryOperator(rc,op, new CastExpression(elementType, lhs).DoResolve(rc), new CastExpression(elementType, rhs).DoResolve(rc));
                    rr = new CastExpression(enumType, rr).DoResolve(rc.WithCheckForOverflow(false));
                    if (rr.IsCompileTimeConstant) // only report result if it's a constant; use the regular OperatorResolveResult codepath otherwise
                        return rr;
                }
            }
            IType resultType = MakeNullable(rc,enumType, isNullable);
            return SetOperationInformations(rc, resultType, lhs, op, rhs, isNullable);
        }

        IType MakeNullable(ResolveContext rc, IType type, bool isNullable)
        {
            if (isNullable)
                return NullableType.Create(rc.compilation, type);
            else
                return type;
        }
        #endregion

        #region BinaryNumericPromotion
        bool BinaryNumericPromotion(ResolveContext rc, bool isNullable, ref Expression lhs, ref Expression rhs, bool allowNullableConstants)
        {
            // V# 4.0 spec: §7.3.6.2
            TypeCode lhsCode = ReflectionHelper.GetTypeCode(NullableType.GetUnderlyingType(lhs.Type));
            TypeCode rhsCode = ReflectionHelper.GetTypeCode(NullableType.GetUnderlyingType(rhs.Type));
            // if one of the inputs is the null literal, promote that to the type of the other operand
            if (isNullable && lhs.Type.Kind == TypeKind.Null && rhsCode >= TypeCode.Boolean && rhsCode <= TypeCode.Decimal)
            {
                lhs = CastTo(rc,rhsCode, isNullable, lhs, allowNullableConstants);
                lhsCode = rhsCode;
            }
            else if (isNullable && rhs.Type.Kind == TypeKind.Null && lhsCode >= TypeCode.Boolean && lhsCode <= TypeCode.Decimal)
            {
                rhs = CastTo(rc,lhsCode, isNullable, rhs, allowNullableConstants);
                rhsCode = lhsCode;
            }
            bool bindingError = false;
            if (lhsCode >= TypeCode.Char && lhsCode <= TypeCode.Decimal
                && rhsCode >= TypeCode.Char && rhsCode <= TypeCode.Decimal)
            {
                TypeCode targetType;
                if (lhsCode == TypeCode.Decimal || rhsCode == TypeCode.Decimal)
                {
                    targetType = TypeCode.Decimal;
                    bindingError = (lhsCode == TypeCode.Single || lhsCode == TypeCode.Double
                                    || rhsCode == TypeCode.Single || rhsCode == TypeCode.Double);
                }
                else if (lhsCode == TypeCode.Double || rhsCode == TypeCode.Double)
                {
                    targetType = TypeCode.Double;
                }
                else if (lhsCode == TypeCode.Single || rhsCode == TypeCode.Single)
                {
                    targetType = TypeCode.Single;
                }
                else if (lhsCode == TypeCode.UInt64 || rhsCode == TypeCode.UInt64)
                {
                    targetType = TypeCode.UInt64;
                    bindingError = IsSigned(lhsCode, lhs) || IsSigned(rhsCode, rhs);
                }
                else if (lhsCode == TypeCode.Int64 || rhsCode == TypeCode.Int64)
                {
                    targetType = TypeCode.Int64;
                }
                else if (lhsCode == TypeCode.UInt32 || rhsCode == TypeCode.UInt32)
                {
                    targetType = (IsSigned(lhsCode, lhs) || IsSigned(rhsCode, rhs)) ? TypeCode.Int64 : TypeCode.UInt32;
                }
                else
                {
                    targetType = TypeCode.Int32;
                }
                lhs = CastTo(rc,targetType, isNullable, lhs, allowNullableConstants);
                rhs = CastTo(rc,targetType, isNullable, rhs, allowNullableConstants);
            }
            return !bindingError;
        }
        bool IsSigned(TypeCode code, Expression rr)
        {
            // Determine whether the rr with code==ReflectionHelper.GetTypeCode(NullableType.GetUnderlyingType(rr.Type))
            // is a signed primitive type.
            switch (code)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                    return true;
                case TypeCode.Int32:
                    // for int, consider implicit constant expression conversion
                    if (rr.IsCompileTimeConstant && rr.ConstantValue != null && (int)rr.ConstantValue >= 0)
                        return false;
                    else
                        return true;
                case TypeCode.Int64:
                    // for long, consider implicit constant expression conversion
                    if (rr.IsCompileTimeConstant && rr.ConstantValue != null && (long)rr.ConstantValue >= 0)
                        return false;
                    else
                        return true;
                default:
                    return false;
            }
        }
        Expression CastTo(ResolveContext rc, TypeCode targetType, bool isNullable, Expression expression, bool allowNullableConstants)
        {
            IType elementType = rc.compilation.FindType(targetType);
            IType nullableType = MakeNullable(rc,elementType, isNullable);
            if (nullableType.Equals(expression.Type))
                return expression;
            if (allowNullableConstants && expression.IsCompileTimeConstant)
            {
                if (expression.ConstantValue == null)
                    return new ConstantExpression(nullableType, null);
                Expression rr = new CastExpression(elementType, expression).DoResolve(rc);
                if (rr.IsError)
                    return rr;
                Debug.Assert(rr.IsCompileTimeConstant);
                return new ConstantExpression(nullableType, rr.ConstantValue);
            }
            else
            {
                return rc.Convert(expression, nullableType,
                               isNullable ? Conversion.ImplicitNullableConversion : Conversion.ImplicitNumericConversion);
            }
        }
        #endregion

        
        #region Null coalescing operator
        Expression ResolveNullCoalescingOperator(ResolveContext rc,Expression lhs, Expression rhs)
        {
            if (NullableType.IsNullable(lhs.Type))
            {
                IType a0 = NullableType.GetUnderlyingType(lhs.Type);
                if (rc.TryConvert(ref rhs, a0))
                    return SetOperationInformations(rc, a0, lhs, BinaryOperatorType.NullCoalescing, rhs);
                
            }
            if (rc.TryConvert(ref rhs, lhs.Type))
            {
                return SetOperationInformations(rc, lhs.Type, lhs, BinaryOperatorType.NullCoalescing, rhs);
            }
            if (rc.TryConvert(ref lhs, rhs.Type))
            {
                return SetOperationInformations(rc, rhs.Type, lhs, BinaryOperatorType.NullCoalescing, rhs);
            }
            else
            {
                return new ErrorExpression(lhs.Type);
            }
        }
        #endregion
        #endregion
		
        //public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        //{
        //    Constant cleft = left.BuilConstantValue(isAttributeConstant) as Constant;
        //    Constant cright = right.BuilConstantValue( isAttributeConstant) as Constant;
        //    if (cleft == null || cright == null)
        //        return null;
        //    return new ConstantBinaryOperator(cleft, oper, cright);
        //}
    }
}