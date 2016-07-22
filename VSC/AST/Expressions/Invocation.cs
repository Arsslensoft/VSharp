using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{  
    /// <summary>
    /// Represents the result of a member invocation.
    /// Used for field/property/event access.
    /// Also, <see cref="InvocationExpression"/> derives from MemberExpressionStatement
    /// </summary>
    public class MemberExpressionStatement : ExpressionStatement
    {
        protected MemberExpressionStatement()
        {
            
        }
        readonly IMember member;
        readonly bool isConstant;
        readonly object constantValue;
        readonly Expression targetResult;
        readonly bool isVirtualCall;

        public MemberExpressionStatement(Expression targetResult, IMember member, IType returnTypeOverride = null)
        {
            this.ResolvedType = returnTypeOverride ?? ComputeType(member);
            _resolved = true;
            this.targetResult = targetResult;
            this.member = member;
            var thisRR = targetResult as SelfReference;
            this.isVirtualCall = member.IsOverridable && !(thisRR != null && thisRR.CausesNonVirtualInvocation);

            IField field = member as IField;
            if (field != null)
            {
                isConstant = field.IsConst;
                if (isConstant)
                    constantValue = field.ConstantValue;
            }
        }

        public MemberExpressionStatement(Expression targetResult, IMember member, bool isVirtualCall, IType returnTypeOverride = null)
        {
            this.ResolvedType = returnTypeOverride ?? ComputeType(member);
            _resolved = true;
            this.targetResult = targetResult;
            this.member = member;
            this.isVirtualCall = isVirtualCall;
            IField field = member as IField;
            if (field != null)
            {
                isConstant = field.IsConst;
                if (isConstant)
                    constantValue = field.ConstantValue;
            }
        }

        static IType ComputeType(IMember member)
        {
            switch (member.SymbolKind)
            {
                case SymbolKind.Constructor:
                    return member.DeclaringType ?? SpecialTypeSpec.UnknownType;
                case SymbolKind.Field:
                    if (((IField)member).IsFixed)
                        return new PointerTypeSpec(member.ReturnType);
                    break;
            }
            return member.ReturnType;
        }

        public MemberExpressionStatement(Expression targetResult, IMember member, IType returnType, bool isConstant, object constantValue)
        {
            this.ResolvedType = returnType;
            _resolved = true;
            this.targetResult = targetResult;
            this.member = member;
            this.isConstant = isConstant;
            this.constantValue = constantValue;
        }
        public MemberExpressionStatement(Expression targetResult, IMember member, IType returnType, bool isConstant, object constantValue, bool isVirtualCall)
        {
            this.ResolvedType = returnType;
            _resolved = true;
            this.targetResult = targetResult;
            this.member = member;
            this.isConstant = isConstant;
            this.constantValue = constantValue;
            this.isVirtualCall = isVirtualCall;
        }

        public Expression TargetResult
        {
            get { return targetResult; }
        }

        /// <summary>
        /// Gets the member.
        /// This property never returns null.
        /// </summary>
        public IMember Member
        {
            get { return member; }
        }

        /// <summary>
        /// Gets whether this MemberResolveResult is a virtual call.
        /// </summary>
        public bool IsVirtualCall
        {
            get { return isVirtualCall; }
        }

        public override bool IsCompileTimeConstant
        {
            get { return isConstant; }
        }

        public override object ConstantValue
        {
            get { return constantValue; }
        }


        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, member);
        }


    }
    /// <summary>
    ///   Invocation of methods or delegates.
    /// </summary>
    public class Invocation : MemberExpressionStatement
    {
        #region Resolved ITEMS

        /// <summary>
        /// Gets the arguments that are being passed to the method, in the order the arguments are being evaluated.
        /// </summary>
        public readonly IList<Expression> RArguments;

        /// <summary>
        /// Gets the list of initializer statements that are appplied to the result of this invocation.
        /// This is used to represent object and collection initializers.
        /// With the initializer statements, the <see cref="InitializedObjectExpression"/> is used
        /// to refer to the result of this invocation.
        /// </summary>
        public readonly IList<Expression> InitializerStatements;

        public Invocation(Expression targetResult, IParameterizedMember member,
                                       IList<Expression> arguments = null,
                                       IList<Expression> initializerStatements = null,
		                               IType returnTypeOverride = null)
			: base(targetResult, member, returnTypeOverride)
		{
            this.RArguments = arguments ?? EmptyList<Expression>.Instance;
            this.InitializerStatements = initializerStatements ?? EmptyList<Expression>.Instance;
		}
		
        #endregion


        protected Arguments arguments;
        protected Expression expr;
        public Invocation(Expression expr, Arguments arguments)
        {
            this.expr = expr;
            this.arguments = arguments;
            if (expr != null)
            {
                loc = expr.Location;
            }
        }
        public Arguments Arguments
        {
            get
            {
                return arguments;
            }
        }
        public Expression Exp
        {
            get
            {
                return expr;
            }
        }

        public new IParameterizedMember Member
        {
            get { return (IParameterizedMember)base.Member; }
        }

        /// <summary>
        /// Gets the arguments in the order they are being passed to the method.
        /// For parameter arrays (params), this will return an ArrayCreateResolveResult.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
                                                         Justification = "Derived methods may be expensive and create new lists")]
        public virtual IList<Expression> GetArgumentsForCall()
        {
            return RArguments;
        }
		

      
    }
}