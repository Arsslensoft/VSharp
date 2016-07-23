using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
using ConstantExpression = VSC.TypeSystem.Resolver.ConstantExpression;

namespace VSC.AST
{
    public abstract class Constant : Expression, IConstantValue
    {
        public Constant
            ResolveType(ICompilation c)
        {
            this._resolved = true;
            this.ResolvedType = c.FindType((this.type as KnownTypeReference).KnownTypeCode);
            return this;
        }
        public virtual AST.Expression Resolve(ResolveContext resolver)
        {
            return new ConstantExpression(type.Resolve(resolver.CurrentTypeResolveContext), GetValue());
        }

        //public override IConstantValue BuilConstantValue(bool isAttributeConstant)
        //{
        //    object val = GetValue();
        //    return new PrimitiveConstantExpression(type, val);
        //}

        public AST.Expression Resolve(ITypeResolveContext context)
        {
            var csContext = (VSharpTypeResolveContext)context;
            if (context.CurrentAssembly != context.Compilation.MainAssembly)
            {
                // The constant needs to be resolved in a different compilation.
                IProjectContent pc = context.CurrentAssembly as IProjectContent;
                if (pc != null)
                {
                    ICompilation nestedCompilation = context.Compilation.SolutionSnapshot.GetCompilation(pc);
                    if (nestedCompilation != null)
                    {
                        var nestedContext = MapToNestedCompilation(csContext, nestedCompilation);
                        AST.Expression rr = Resolve(new ResolveContext(nestedContext, CompilerContext.report));
                        return MapToNewContext(rr, context);
                    }
                }
            }
            // Resolve in current context.
            return Resolve(new ResolveContext(csContext, CompilerContext.report));
        }

        VSharpTypeResolveContext MapToNestedCompilation(VSharpTypeResolveContext context, ICompilation nestedCompilation)
        {
            var nestedContext = new VSharpTypeResolveContext(nestedCompilation.MainAssembly);
            if (context.CurrentUsingScope != null)
            {
                nestedContext = nestedContext.WithUsingScope(context.CurrentUsingScope.UnresolvedUsingScope.ResolveScope(nestedCompilation));
            }
            if (context.CurrentTypeDefinition != null)
            {
                nestedContext = nestedContext.WithCurrentTypeDefinition(nestedCompilation.Import(context.CurrentTypeDefinition));
            }
            return nestedContext;
        }

        static AST.Expression MapToNewContext(AST.Expression rr, ITypeResolveContext newContext)
        {
            if (rr is TypeOfExpression)
            {
                return new TypeOfExpression(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    ((TypeOfExpression)rr).TargetType.ToTypeReference().Resolve(newContext));
            }
            else if (rr is ArrayCreateExpression)
            {
                ArrayCreateExpression acrr = (ArrayCreateExpression)rr;
                return new ArrayCreateExpression(
                    acrr.Type.ToTypeReference().Resolve(newContext),
                    MapToNewContext(acrr.SizeArguments, newContext),
                    MapToNewContext(acrr.InitializerElements, newContext));
            }
            else if (rr.IsCompileTimeConstant)
            {
                return new ConstantExpression(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    rr.ConstantValue
                );
            }
            else
            {
                return new ErrorExpression(rr.Type.ToTypeReference().Resolve(newContext));
            }
        }

        static AST.Expression[] MapToNewContext(IList<AST.Expression> input, ITypeResolveContext newContext)
        {
            if (input == null)
                return null;
            AST.Expression[] output = new AST.Expression[input.Count];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = MapToNewContext(input[i], newContext);
            }
            return output;
        }


        static readonly NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

        protected Constant(Location loc)
        {
            this.loc = loc;
        }
        protected Constant()
        {
            this.loc = Location.Null;
        }
        override public string ToString()
        {
            return this.GetType().Name + " (" + GetValueAsLiteral() + ")";
        }

        /// <summary>
        ///  This is used to obtain the actual value of the literal
        ///  cast into an object.
        /// </summary>
        public abstract object GetValue();

        public abstract long GetValueAsLong();

        public abstract string GetValueAsLiteral();


        public abstract bool IsDefaultValue
        {
            get;
        }

        public abstract bool IsNegative
        {
            get;
        }

        //
        // When constant is declared as literal
        //
        public virtual bool IsLiteral
        {
            get { return false; }
        }

        public virtual bool IsOneInteger
        {
            get { return false; }
        }

     
        public virtual bool IsZeroInteger
        {
            get { return false; }
        }
        public override Expression DoResolve(ResolveContext rc)
        {
            if (_resolved)
                return this;

            ResolvedType = type.Resolve(rc);
            _resolved = true;
            return this;
        }
        public static Constant CreateConstantFromValue(ResolveContext rc,IType t, object v, Location loc)
        {
            Constant c = null;
            switch ((t as ITypeDefinition).KnownTypeCode)
            {
                    
                case KnownTypeCode.Int32:
                    c=  new IntConstant( (int)v, loc);
                    break;
                case KnownTypeCode.String:
                    c = new StringConstant((string)v, loc);   break;
                case KnownTypeCode.UInt32:
                    c = new UIntConstant((uint)v, loc);   break;
                case KnownTypeCode.Int64:
                    c = new LongConstant((long)v, loc);   break;
                case KnownTypeCode.UInt64:
                    c = new ULongConstant((ulong)v, loc);   break;
                case KnownTypeCode.Single:
                    c = new FloatConstant((float)v, loc);   break;
                case KnownTypeCode.Double:
                    c = new DoubleConstant((double)v, loc);   break;
                case KnownTypeCode.Int16:
                    c = new ShortConstant((short)v, loc);   break;
                case KnownTypeCode.UInt16:
                    c = new UShortConstant((ushort)v, loc);   break;
                case KnownTypeCode.SByte:
                    c = new SByteConstant((sbyte)v, loc);   break;
                case KnownTypeCode.Byte:
                    c = new ByteConstant((byte)v, loc);   break;
                case KnownTypeCode.Char:
                    c = new CharConstant((char)v, loc);   break;
                case KnownTypeCode.Boolean:
                    c = new BoolConstant((bool)v, loc);   break;
            }
       

            if (t.Kind == TypeKind.Enum)
            {
                var real_type = ResolveContext.GetEnumUnderlyingType(t);
                  return CreateConstantFromValue(rc,real_type, v, loc);
            }

            if (v == null)
            {
             // TODO:Support nullable constant ?

                if (t.IsReferenceType.HasValue && t.IsReferenceType.Value)
                    c= new NullConstant(t, loc);
            }

            if (c != null)
                c = (Constant)c.DoResolve(rc);

            return c;

        }
        public static Constant CreateConstantFromValue(ICompilation rc, IType t, object v, Location loc)
        {
            Constant c = null;
            switch ((t as ITypeDefinition).KnownTypeCode)
            {

                case KnownTypeCode.Int32:
                    c = new IntConstant((int)v, loc);
                    break;
                case KnownTypeCode.String:
                    c = new StringConstant((string)v, loc); break;
                case KnownTypeCode.UInt32:
                    c = new UIntConstant((uint)v, loc); break;
                case KnownTypeCode.Int64:
                    c = new LongConstant((long)v, loc); break;
                case KnownTypeCode.UInt64:
                    c = new ULongConstant((ulong)v, loc); break;
                case KnownTypeCode.Single:
                    c = new FloatConstant((float)v, loc); break;
                case KnownTypeCode.Double:
                    c = new DoubleConstant((double)v, loc); break;
                case KnownTypeCode.Int16:
                    c = new ShortConstant((short)v, loc); break;
                case KnownTypeCode.UInt16:
                    c = new UShortConstant((ushort)v, loc); break;
                case KnownTypeCode.SByte:
                    c = new SByteConstant((sbyte)v, loc); break;
                case KnownTypeCode.Byte:
                    c = new ByteConstant((byte)v, loc); break;
                case KnownTypeCode.Char:
                    c = new CharConstant((char)v, loc); break;
                case KnownTypeCode.Boolean:
                    c = new BoolConstant((bool)v, loc); break;
            }


            if (t.Kind == TypeKind.Enum)
            {
                var real_type = ResolveContext.GetEnumUnderlyingType(t);
                return CreateConstantFromValue(rc, real_type, v, loc);
            }

            if (v == null)
            {
                // TODO:Support nullable constant ?

                if (t.IsReferenceType.HasValue && t.IsReferenceType.Value)
                    c = new NullConstant(t, loc);
            }

            if (c != null)
                c = ((Constant) c).ResolveType(rc);

            return c;

        }
    }
 
    [Serializable]
    public sealed class ConstantCast : Constant, ISupportsInterning
    {
        readonly ITypeReference targetType;
        readonly Constant expression;
        readonly bool allowNullableConstants;

        public ConstantCast(ITypeReference targetType, Constant expression, bool allowNullableConstants)
            :base(expression.Location)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            if (expression == null)
                throw new ArgumentNullException("expression");
            this.targetType = targetType;
            this.expression = expression;
            this.allowNullableConstants = allowNullableConstants;
        }

        public override AST.Expression Resolve(ResolveContext resolver)
        {
            var type = targetType.Resolve(resolver.CurrentTypeResolveContext);
            var resolveResult = expression.Resolve(resolver);
            //if (allowNullableConstants && NullableType.IsNullable(type))
            //{
            //    resolveResult = resolver.ResolveCast(NullableType.GetUnderlyingType(type), resolveResult);
            //    if (resolveResult.IsCompileTimeConstant)
            //        return new ConstantExpression(type, resolveResult.ConstantValue);
            //}
            //return resolver.ResolveCast(type, resolveResult);
            return ErrorResult;
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            unchecked
            {
                return targetType.GetHashCode() + expression.GetHashCode() * 1018829;
            }
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            ConstantCast cast = other as ConstantCast;
            return cast != null
                && this.targetType == cast.targetType && this.expression == cast.expression && this.allowNullableConstants == cast.allowNullableConstants;
        }

        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override long GetValueAsLong()
        {
            throw new NotImplementedException();
        }

        public override string GetValueAsLiteral()
        {
            throw new NotImplementedException();
        }

        public override bool IsDefaultValue
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsNegative
        {
            get { throw new NotImplementedException(); }
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

        public AST.Expression Resolve(ITypeResolveContext context)
        {
            AST.Expression rr = baseValue.Resolve(context);
            if (rr.IsCompileTimeConstant && rr.ConstantValue != null)
            {
                object val = rr.ConstantValue;
                TypeCode typeCode = Type.GetTypeCode(val.GetType());
                if (typeCode >= TypeCode.SByte && typeCode <= TypeCode.UInt64)
                {
                    long intVal = (long)VSharpPrimitiveCast.Cast(TypeCode.Int64, val, false);
                    object newVal = VSharpPrimitiveCast.Cast(typeCode, unchecked(intVal + incrementAmount), false);
                    return new ConstantExpression(rr.Type, newVal);
                }
            }
            return new ErrorExpression(rr.Type);
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

    /// <summary>
    /// V#'s equivalent to the SimpleConstantValue.
    /// </summary>
    [Serializable]
    public sealed class PrimitiveConstantExpression : Constant, ISupportsInterning
    {
        readonly ITypeReference type;
        readonly object value;

        public ITypeReference Type
        {
            get { return type; }
        }

        public object Value
        {
            get { return value; }
        }

        public PrimitiveConstantExpression(ITypeReference type, object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            this.type = type;
            this.value = value;
        }

        public override AST.Expression Resolve(ResolveContext resolver)
        {
            return new ConstantExpression(type.Resolve(resolver.CurrentTypeResolveContext), value);
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            return type.GetHashCode() ^ (value != null ? value.GetHashCode() : 0);
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            PrimitiveConstantExpression scv = other as PrimitiveConstantExpression;
            return scv != null && type == scv.type && value == scv.value;
        }

        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override long GetValueAsLong()
        {
            throw new NotImplementedException();
        }

        public override string GetValueAsLiteral()
        {
            throw new NotImplementedException();
        }

        public override bool IsDefaultValue
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsNegative
        {
            get { throw new NotImplementedException(); }
        }
    }

    //[Serializable]
    //public sealed class TypeOfConstantExpression : Constant
    //{
    //    readonly ITypeReference type;

    //    public ITypeReference Type
    //    {
    //        get { return type; }
    //    }

    //    public TypeOfConstantExpression(ITypeReference type)
    //    {
    //        this.type = type;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        return resolver.ResolveTypeOf(type.Resolve(resolver.CurrentTypeResolveContext));
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantIdentifierReference : Constant
    //{
    //    readonly string identifier;
    //    readonly IList<ITypeReference> typeArguments;

    //    public ConstantIdentifierReference(string identifier, IList<ITypeReference> typeArguments = null)
    //    {
    //        if (identifier == null)
    //            throw new ArgumentNullException("identifier");
    //        this.identifier = identifier;
    //        this.typeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
          
    //        return resolver.ResolveSimpleName(identifier, typeArguments.Resolve(resolver.CurrentTypeResolveContext));
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantMemberReference : Constant
    //{
    //    readonly ITypeReference targetType;
    //    readonly Constant targetExpression;
    //    readonly string memberName;
    //    readonly IList<ITypeReference> typeArguments;

    //    public ConstantMemberReference(ITypeReference targetType, string memberName, IList<ITypeReference> typeArguments = null)
    //    {
    //        List<int> l = new List<int>() {1,2,3,4,5};
    //        if (targetType == null)
    //            throw new ArgumentNullException("targetType");
    //        if (memberName == null)
    //            throw new ArgumentNullException("memberName");
    //        this.targetType = targetType;
    //        this.memberName = memberName;
    //        this.typeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
    //    }

    //    public ConstantMemberReference(Constant targetExpression, string memberName, IList<ITypeReference> typeArguments = null)
    //    {
    //        if (targetExpression == null)
    //            throw new ArgumentNullException("targetExpression");
    //        if (memberName == null)
    //            throw new ArgumentNullException("memberName");
    //        this.targetExpression = targetExpression;
    //        this.memberName = memberName;
    //        this.typeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        AST.Expression rr;
    //        if (targetType != null)
    //            rr = new TypeExpression(targetType.Resolve(resolver.CurrentTypeResolveContext));
    //        else
    //            rr = targetExpression.Resolve(resolver);
    //        return resolver.ResolveMemberAccess(rr, memberName, typeArguments.Resolve(resolver.CurrentTypeResolveContext));
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantCheckedExpression : Constant
    //{
    //    readonly bool checkForOverflow;
    //    readonly Constant expression;

    //    public ConstantCheckedExpression(bool checkForOverflow, Constant expression)
    //    {
    //        if (expression == null)
    //            throw new ArgumentNullException("expression");
    //        this.checkForOverflow = checkForOverflow;
    //        this.expression = expression;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        return expression.Resolve(resolver.WithCheckForOverflow(checkForOverflow));
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantDefaultValue : Constant, ISupportsInterning
    //{
    //    readonly ITypeReference type;

    //    public ConstantDefaultValue(ITypeReference type)
    //    {
    //        if (type == null)
    //            throw new ArgumentNullException("type");
    //        this.type = type;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        return resolver.ResolveDefaultValue(type.Resolve(resolver.CurrentTypeResolveContext));
    //    }

    //    int ISupportsInterning.GetHashCodeForInterning()
    //    {
    //        return type.GetHashCode();
    //    }

    //    bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
    //    {
    //        ConstantDefaultValue o = other as ConstantDefaultValue;
    //        return o != null && this.type == o.type;
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantUnaryOperator : Constant
    //{
    //    readonly UnaryOperatorType operatorType;
    //    readonly Constant expression;

    //    public ConstantUnaryOperator(UnaryOperatorType operatorType, Constant expression)
    //    {
    //        if (expression == null)
    //            throw new ArgumentNullException("expression");
    //        this.operatorType = operatorType;
    //        this.expression = expression;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        return resolver.ResolveUnaryOperator(operatorType, expression.Resolve(resolver));
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantBinaryOperator : Constant
    //{
    //    readonly Constant left;
    //    readonly BinaryOperatorType operatorType;
    //    readonly Constant right;

    //    public ConstantBinaryOperator(Constant left, BinaryOperatorType operatorType, Constant right)
    //    {
    //        if (left == null)
    //            throw new ArgumentNullException("left");
    //        if (right == null)
    //            throw new ArgumentNullException("right");
    //        this.left = left;
    //        this.operatorType = operatorType;
    //        this.right = right;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        AST.Expression lhs = left.Resolve(resolver);
    //        AST.Expression rhs = right.Resolve(resolver);
    //        return resolver.ResolveBinaryOperator(operatorType, lhs, rhs);
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    //[Serializable]
    //public sealed class ConstantConditionalOperator : Constant
    //{
    //    readonly Constant condition, trueExpr, falseExpr;

    //    public ConstantConditionalOperator(Constant condition, Constant trueExpr, Constant falseExpr)
    //    {
    //        if (condition == null)
    //            throw new ArgumentNullException("condition");
    //        if (trueExpr == null)
    //            throw new ArgumentNullException("trueExpr");
    //        if (falseExpr == null)
    //            throw new ArgumentNullException("falseExpr");
    //        this.condition = condition;
    //        this.trueExpr = trueExpr;
    //        this.falseExpr = falseExpr;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
           
    //        return resolver.ResolveConditional(
    //            condition.Resolve(resolver),
    //            trueExpr.Resolve(resolver),
    //            falseExpr.Resolve(resolver)
    //        );
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}
    ///// <summary>
    ///// Represents an array creation (as used within an attribute argument)
    ///// </summary>
    //[Serializable]
    //public sealed class ConstantArrayCreation : Constant
    //{
    //    // type may be null when the element is being inferred
    //    readonly ITypeReference elementType;
    //    readonly IList<Constant> arrayElements;

    //    public ConstantArrayCreation(ITypeReference type, IList<Constant> arrayElements)
    //    {
    //        if (arrayElements == null)
    //            throw new ArgumentNullException("arrayElements");
    //        this.elementType = type;
    //        this.arrayElements = arrayElements;
    //    }

    //    public override AST.Expression Resolve(ResolveContext resolver)
    //    {
    //        AST.Expression[] elements = new AST.Expression[arrayElements.Count];
    //        for (int i = 0; i < elements.Length; i++)
    //        {
    //            elements[i] = arrayElements[i].Resolve(resolver);
    //        }
    //        int[] sizeArguments = { elements.Length };
    //        if (elementType != null)
    //        {
    //            return resolver.ResolveArrayCreation(elementType.Resolve(resolver.CurrentTypeResolveContext), sizeArguments, elements);
    //        }
    //        else
    //        {
    //            return resolver.ResolveArrayCreation(null, sizeArguments, elements);
    //        }
    //    }

    //    public override object GetValue()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override long GetValueAsLong()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string GetValueAsLiteral()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsDefaultValue
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override bool IsNegative
    //    {
    //        get { throw new NotImplementedException(); }
    //    }
    //}

  

}
