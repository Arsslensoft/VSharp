using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    /// Used for constants that could not be converted to IConstantValue.
    /// </summary>
    [Serializable]
    public sealed class ErrorConstantValue : IConstantValue
    {
        readonly ITypeReference type;

        public ErrorConstantValue(ITypeReference type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            this.type = type;
        }

        public ResolveResult Resolve(ITypeResolveContext context)
        {
            return new ErrorResolveResult(type.Resolve(context));
        }
    }
    /// <summary>
    /// Increments an integer <see cref="IConstantValue"/> by a fixed amount without changing the type.
    /// </summary>
    [Serializable]
    public sealed class IncrementConstantValue : IConstantValue, ISupportsInterning
    {
        readonly IConstantValue baseValue;
        readonly int incrementAmount;

        public IncrementConstantValue(IConstantValue baseValue, int incrementAmount = 1)
        {
            if (baseValue == null)
                throw new ArgumentNullException("baseValue");
            IncrementConstantValue icv = baseValue as IncrementConstantValue;
            if (icv != null)
            {
                this.baseValue = icv.baseValue;
                this.incrementAmount = icv.incrementAmount + incrementAmount;
            }
            else
            {
                this.baseValue = baseValue;
                this.incrementAmount = incrementAmount;
            }
        }

        public ResolveResult Resolve(ITypeResolveContext context)
        {
            ResolveResult rr = baseValue.Resolve(context);
            if (rr.IsCompileTimeConstant && rr.ConstantValue != null)
            {
                object val = rr.ConstantValue;
                TypeCode typeCode = System.Type.GetTypeCode(val.GetType());
                if (typeCode >= TypeCode.SByte && typeCode <= TypeCode.UInt64)
                {
                    long intVal = (long)VSharpPrimitiveCast.Cast(TypeCode.Int64, val, false);
                    object newVal = VSharpPrimitiveCast.Cast(typeCode, unchecked(intVal + incrementAmount), false);
                    return new ConstantResolveResult(rr.Type, newVal);
                }
            }
            return new ErrorResolveResult(rr.Type);
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            unchecked
            {
                return baseValue.GetHashCode() * 33 ^ incrementAmount;
            }
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            IncrementConstantValue o = other as IncrementConstantValue;
            return o != null && baseValue == o.baseValue && incrementAmount == o.incrementAmount;
        }
    }
    [Serializable]
    public sealed class ConstantCast : ConstantExpression, ISupportsInterning
    {
        readonly ConstantExpression expression;
        readonly bool allowNullableConstants;

        public ConstantCast(ITypeReference targetType, ConstantExpression expression, bool allowNullableConstants) : base(targetType, null, new Base.GoldParser.Parser.LineInfo())
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            if (expression == null)
                throw new ArgumentNullException("expression");
            this.expression = expression;
            this.allowNullableConstants = allowNullableConstants;
        }

        public override ResolveResult Resolve(VSharpResolver resolver)
        {
            var type = base.Type.Resolve(resolver.CurrentTypeResolveContext);
            var resolveResult = expression.Resolve(resolver);
            if (allowNullableConstants && NullableType.IsNullable(type))
            {
                resolveResult = resolver.ResolveCast(NullableType.GetUnderlyingType(type), resolveResult);
                if (resolveResult.IsCompileTimeConstant)
                    return new ConstantResolveResult(type, resolveResult.ConstantValue);
            }
            return resolver.ResolveCast(type, resolveResult);
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            unchecked
            {
                return base.Type.GetHashCode() + expression.GetHashCode() * 1018829;
            }
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            ConstantCast cast = other as ConstantCast;
            return cast != null
                && base.Type == cast.Type && this.expression == cast.expression && this.allowNullableConstants == cast.allowNullableConstants;
        }
    }
}
