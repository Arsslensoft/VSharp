using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{

    /// <summary>
    /// A cast expression that represents a type conversion
    /// </summary>
    public class CastExpression : ShimExpression, IConstantValue
    {
        Expression target_type;

        public CastExpression(Expression cast_type, Expression expr, Location loc)
            : base(expr)
        {
            this.target_type = cast_type;
            this.loc = loc;
        }
        public readonly Conversion Conversion;

      
        // resolved ctor
        public readonly bool CheckForOverflow=false;
        public CastExpression(IType targetType, Expression input, Conversion conversion)
            : base(input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (conversion == null)
				throw new ArgumentNullException("conversion");
			this.Conversion = conversion;
            this.ResolvedType = targetType;
            this._resolved = true;
		}
        public CastExpression(IType targetType, Expression input)
            : base(input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            this.ResolvedType = targetType;

        }
        public CastExpression(IType targetType, Expression input,Location l)
            : base(input)
        {
            loc = l;
            if (input == null)
                throw new ArgumentNullException("input");
            this.ResolvedType = targetType;

        }
        public CastExpression(IType targetType, Expression input, Conversion conversion, bool checkForOverflow)
			: this(targetType, input, conversion)
		{
			this.CheckForOverflow = checkForOverflow;
		}
        public CastExpression(IType targetType, Expression input, Conversion conversion, bool checkForOverflow, Location l)
            : this(targetType, input, conversion, checkForOverflow)
        {
            this.loc = l;
        }

        public Expression TargetType
        {
            get { return target_type; }
        }


        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;
         
            expr = expr.DoResolve(rc);
            if (expr == null)
                return null;
 
            eclass = ExprClass.Value;
            if (ResolvedType == null)
            {
                ResolvedType = target_type.ResolveAsType(rc);


                if (ResolvedType == null)
                    return null;
            }

            if (rc.IsStaticType(ResolvedType))
            {
                rc.Report.Error(246, loc, "Cannot convert to static type `{0}'", ResolvedType.ToString());
                return null;
            }
          

            // V# 4.0 spec: §7.7.6 Cast expressions
            Conversion c = rc.conversions.ExplicitConversion(expr, ResolvedType);

            if (expr.IsCompileTimeConstant && !c.IsUserDefined)
            {
                TypeCode code = ReflectionHelper.GetTypeCode(ResolvedType);
                if (code >= TypeCode.Boolean && code <= TypeCode.Decimal && expr.ConstantValue != null)
                {
                    try
                    {
                        return Constant.CreateConstantFromValue(rc,ResolvedType, rc.VSharpPrimitiveCast(code, expr.ConstantValue),loc);
                    }
                    catch (OverflowException)
                    {
                        return new ErrorExpression(ResolvedType, loc);
                    }
                    catch (InvalidCastException)
                    {
                        return new ErrorExpression(ResolvedType, loc);
                    }
                }
                else if (code == TypeCode.String)
                {
                    if (expr.ConstantValue == null || expr.ConstantValue is string)
                        return Constant.CreateConstantFromValue(rc, ResolvedType,  expr.ConstantValue, loc);
                    else
                        return new ErrorExpression(ResolvedType, loc);
                }
                else if (ResolvedType.Kind == TypeKind.Enum)
                {
                    code = ReflectionHelper.GetTypeCode(ResolveContext.GetEnumUnderlyingType(ResolvedType));
                    if (code >= TypeCode.SByte && code <= TypeCode.UInt64 && expr.ConstantValue != null)
                    {
                        try
                        {
                            return Constant.CreateConstantFromValue(rc, ResolvedType, rc.VSharpPrimitiveCast(code, expr.ConstantValue), loc);
                        }
                        catch (OverflowException)
                        {
                            return new ErrorExpression(ResolvedType, loc);
                        }
                        catch (InvalidCastException)
                        {
                            return new ErrorExpression(ResolvedType,loc);
                        }
                    }
                }
            }
            return new CastExpression(ResolvedType, expr, c, rc.checkForOverflow);
        }
        //public override IConstantValue BuilConstantValue( bool isAttributeConstant)
        //{
        //    Constant v = Expr.BuilConstantValue( isAttributeConstant) as Constant;
        //    if (v == null)
        //        return null;
        //    var typeReference = TargetType as ITypeReference;
        //    return new ConstantCast(typeReference, v, false);
        //}
        public override VSC.AST.Expression Constantify(VSC.TypeSystem.Resolver.ResolveContext resolver)
        {
            return DoResolve(resolver) as Constant;
        }
        

    }
}