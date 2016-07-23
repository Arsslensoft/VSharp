using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        protected IMember member;
        protected readonly bool isConstant;
        protected readonly object constantValue;
     protected   Expression targetResult;
     protected bool isVirtualCall;

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

       internal static IType ComputeType(IMember member)
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
    public class Invocation : ExpressionStatement
    {
        #region Resolved
        	/// <summary>
		/// Gets the arguments that are being passed to the method, in the order the arguments are being evaluated.
		/// </summary>
        public readonly IList<AST.Expression> RArguments;
		
		/// <summary>
		/// Gets the list of initializer statements that are appplied to the result of this invocation.
		/// This is used to represent object and collection initializers.
		/// With the initializer statements, the <see cref="InitializedObjectExpression"/> is used
		/// to refer to the result of this invocation.
		/// </summary>
        public readonly IList<AST.Expression> InitializerStatements;
        protected IMember member;
        protected Expression targetResult;
        protected bool isVirtualCall;

        public readonly OverloadResolutionErrors OverloadResolutionErrors;

        /// <summary>
        /// Gets whether this invocation is calling an extension method using extension method syntax.
        /// </summary>
        public readonly bool IsExtensionMethodInvocation;

        /// <summary>
        /// Gets whether this invocation is calling a delegate (without explicitly calling ".Invoke()").
        /// </summary>
        public readonly bool IsDelegateInvocation;

        /// <summary>
        /// Gets whether a params-Array is being used in its expanded form.
        /// </summary>
        public readonly bool IsExpandedForm;

        readonly IList<int> argumentToParameterMap;
        public Invocation(Expression targetResult, IParameterizedMember member,
            IList<Expression> arguments,
            OverloadResolutionErrors overloadResolutionErrors = OverloadResolutionErrors.None,
            bool isExtensionMethodInvocation = false,
            bool isExpandedForm = false,
            bool isDelegateInvocation = false,
            IList<int> argumentToParameterMap = null,
            IList<Expression> initializerStatements = null,
            IType returnTypeOverride = null)
		{
            this.ResolvedType = returnTypeOverride ?? MemberExpressionStatement.ComputeType(member);
            _resolved = true;
            this.targetResult = targetResult;
            this.member = member;
            this.RArguments = arguments ?? EmptyList<AST.Expression>.Instance;
            this.InitializerStatements = initializerStatements ?? EmptyList<AST.Expression>.Instance;
            this.OverloadResolutionErrors = overloadResolutionErrors;
            this.IsExtensionMethodInvocation = isExtensionMethodInvocation;
            this.IsExpandedForm = isExpandedForm;
            this.IsDelegateInvocation = isDelegateInvocation;
            this.argumentToParameterMap = argumentToParameterMap;
            eclass = ExprClass.Value;
		}
		
		public new IParameterizedMember Member {
			get { return (IParameterizedMember)member; }
		}

        /// <summary>
        /// Gets an array that maps argument indices to parameter indices.
        /// For arguments that could not be mapped to any parameter, the value will be -1.
        /// 
        /// parameterIndex = ArgumentToParameterMap[argumentIndex]
        /// </summary>
        public IList<int> GetArgumentToParameterMap()
        {
            return argumentToParameterMap;
        }

        public IList<Expression> GetArgumentsForCall()
        {
            Expression[] results = new Expression[Member.Parameters.Count];
            List<Expression> paramsArguments = IsExpandedForm ? new List<Expression>() : null;
            // map arguments to parameters:
            for (int i = 0; i < Arguments.Count; i++)
            {
                int mappedTo;
                if (argumentToParameterMap != null)
                    mappedTo = argumentToParameterMap[i];
                else
                    mappedTo = IsExpandedForm ? Math.Min(i, results.Length - 1) : i;

                if (mappedTo >= 0 && mappedTo < results.Length)
                {
                    if (IsExpandedForm && mappedTo == results.Length - 1)
                    {
                        paramsArguments.Add(RArguments[i]);
                    }
                    else
                    {
                        var narr = RArguments[i] as NamedArgumentExpression;
                        if (narr != null)
                            results[mappedTo] = narr.Argument;
                        else
                            results[mappedTo] = RArguments[i];
                    }
                }
            }
            if (IsExpandedForm)
            {
                IType arrayType = Member.Parameters.Last().Type;
                IType int32 = Member.Compilation.FindType(KnownTypeCode.Int32);
                Expression[] sizeArguments = {Constant.CreateConstantFromValue(Member.Compilation,int32, paramsArguments.Count, Location.Null) };
                results[results.Length - 1] = new ArrayCreateExpression(arrayType, sizeArguments, paramsArguments);
            }

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] == null)
                {
                    if (Member.Parameters[i].IsOptional)
                    {
                        results[i] = Constant.CreateConstantFromValue(Member.Compilation, Member.Parameters[i].Type, Member.Parameters[i].ConstantValue, Location.Null);
                    }
                    else
                    {
                        results[i] = ErrorExpression.UnknownError;
                    }
                }
            }

            return results;
        }
        #endregion

        #region Unresolved
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
        #endregion


        public override Expression DoResolve(ResolveContext rc)
        {
            eclass = ExprClass.Value;
            MemberAccess mre = expr as MemberAccess;
            SimpleName typeorvar = mre != null ? mre.LeftExpression as SimpleName : null;
            if (typeorvar != null && typeorvar.TypeArguments.Count == 0)
            {
                // Special handling for §7.6.4.1 Identicial simple names and type names
                typeorvar.lookupMode = NameLookupMode.Expression;
                Expression idRR = typeorvar.DoResolve(rc);
                Expression target = ResolveMemberReferenceOnGivenTarget(rc,idRR, mre);
                TypeExpression trr;
                if (rc.IsVariableReferenceWithSameType(idRR, out trr))
                {
                    // It's ambiguous
                    Expression rr = ResolveInvocationOnGivenTarget(rc,target,arguments);
                    Expression simpleNameRR = IsStaticResult(target, rr) ? trr : idRR;
                    rc.Report.Warning(0,1,loc, "Ambiguous simple name '{0}' was resolved to {1}",
                                  typeorvar.GetSignatureForError(), simpleNameRR.GetSignatureForError());
                    return rr;
                }
                else
                {
                    // It's not ambiguous
                    return ResolveInvocationOnGivenTarget(rc,target,arguments);
                }
            }

            return this;
        }
        #region ResolveInvocation
        /// <summary>
        /// Resolves an invocation.
        /// </summary>
        /// <param name="target">The target of the invocation. Usually a MethodGroupResolveResult.</param>
        /// <param name="arguments">
        /// Arguments passed to the method.
        /// The resolver may mutate this array to wrap elements in <see cref="ConversionResolveResult"/>s!
        /// </param>
        /// <param name="argumentNames">
        /// The argument names. Pass the null string for positional arguments.
        /// </param>
        /// <returns>InvocationExpression</returns>
        private Expression ResolveInvocation(ResolveContext rc,Expression target, Expression[] arguments, string[] argumentNames, bool allowOptionalParameters)
        {
            // C# 4.0 spec: §7.6.5
            MethodGroupExpression mgrr = target as MethodGroupExpression;
            if (mgrr != null)
            {

                OverloadResolution or = mgrr.PerformOverloadResolution(rc.compilation, arguments, argumentNames, checkForOverflow: rc.checkForOverflow, conversions: rc.conversions, allowOptionalParameters: allowOptionalParameters);
                if (or.BestCandidate != null)
                {

                    var m = or.BestCandidate;
                    
                    if (arguments == null && m.Name == DestructorDeclaration.MetadataName) 
				            rc.Report.Error (0, loc, "Destructors cannot be called directly. Consider calling IDisposable.Dispose if available");

                    CheckSpecialMethod(rc, m);


                    if (or.BestCandidate.IsStatic && !or.IsExtensionMethodInvocation && !(mgrr.TargetResult is TypeExpression))
                        return or.CreateInvocation(new TypeExpression(mgrr.TargetType), returnTypeOverride:  null);
                    else
                        return or.CreateInvocation(mgrr.TargetResult, returnTypeOverride: null);
                }
                else
                {
                    // No candidate found at all (not even an inapplicable one).
                    // This can happen with empty method groups (as sometimes used with extension methods)
                    rc.Report.Error(0, loc, "`{0}' does not contain a definition for `{1}'",
            mgrr.TargetType.ToString(), mgrr.MethodName);
                    return null;
                }
            }
            if (target == null && expr is SimpleName)
            {
                rc.Report.Error(0, loc, "`{0}' does not contain a definition for `{1}'",
   rc.CurrentTypeDefinition.ToString(), expr.GetSignatureForError());
                return null;
            }
            else if(target == null)
                return null;
            
            
            IMethod invokeMethod = target.Type.GetDelegateInvokeMethod();
            if (invokeMethod != null)
            {  
                // is it a delegate ?
                if (target.Type.Kind != TypeKind.Delegate)
                {
                    rc.Report.Error(0, loc, "Cannot invoke a non-delegate type `{0}'",
                          target.Type.ToString());
                    return null;

                }
                OverloadResolution or = rc.CreateOverloadResolution(arguments, argumentNames);
                or.AddCandidate(invokeMethod);

                return new Invocation(
                    target, invokeMethod, //invokeMethod.ReturnType.Resolve(context),
                    or.GetArgumentsWithConversionsAndNames(), or.BestCandidateErrors,
                    isExpandedForm: or.BestCandidateIsExpandedForm,
                    isDelegateInvocation: true,
                    argumentToParameterMap: or.GetArgumentToParameterMap(),
                    returnTypeOverride: null);
            }

            rc.Report.Error(0, loc, "The member `{0}' cannot be used as method or delegate",
                    target.GetSignatureForError());

            return ErrorResult;
        }

        void CheckSpecialMethod(ResolveContext rc, IParameterizedMember m)
        {
            if(m.SymbolKind == SymbolKind.Accessor ||  m.SymbolKind == SymbolKind.Operator)
            rc.Report.Error(0, loc, "`{0}': cannot explicitly call operator or accessor",
    m.ToString());
        }
   
    

        #endregion


        public void ReportOverloadErrors( OverloadResolutionErrors errors, Location l)
        {
           
            //if ((errors & OverloadResolutionErrors.WrongNumberOfTypeArguments) != 0)
            //    rc.Report.Error(0,l, "The method {0} has wrong number of type arguments, {1} type arguments expected.", bc.);

        }
        /// <summary>
        /// Gets whether 'rr' is considered a static access on the target identifier.
        /// </summary>
        /// <param name="rr">Resolve Result of the MemberReferenceExpression</param>
        /// <param name="invocationRR">Resolve Result of the InvocationExpression</param>
        bool IsStaticResult(Expression rr, Expression invocationRR)
        {
            if (rr is TypeExpression)
                return true;
            MemberExpressionStatement mrr = (rr is MethodGroupExpression ? invocationRR : rr) as MemberExpressionStatement;
            return mrr != null && mrr.Member.IsStatic;
        }
        Expression ResolveInvocationOnGivenTarget(ResolveContext rc,Expression target, Arguments args)
        {
            string[] argumentNames;
            if (args != null)
                args.Resolve(rc);


            Expression[] arguments = args.GetArguments(out argumentNames);
            return ResolveInvocation(rc, target, arguments, argumentNames, true);
    
        }
        Expression ResolveMemberReferenceOnGivenTarget(ResolveContext rc,Expression target, MemberAccess memberReferenceExpression)
        {
            return
                new MemberAccess(target, memberReferenceExpression.Name, memberReferenceExpression.TypeArguments,
                    memberReferenceExpression.Location, NameLookupMode.Expression).DoResolve(rc);
          
        }
    }
}