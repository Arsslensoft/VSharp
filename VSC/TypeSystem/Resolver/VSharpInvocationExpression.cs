using System;
using System.Collections.Generic;
using System.Linq;
using VSC.AST;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the result of a method, constructor or indexer invocation.
	/// Provides additional V#-specific information for InvocationResolveResult.
	/// </summary>
    public class VSharpInvocationExpression : Invocation
	{
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

		/// <summary>
		/// If IsExtensionMethodInvocation is true this property holds the reduced method.
		/// </summary>
        //IMethod reducedMethod;
        //public IMethod ReducedMethod {
        //    get {
        //        if (!IsExtensionMethodInvocation)
        //            return null;
        //        if (reducedMethod == null && Member is IMethod)
        //            reducedMethod = new ReducedExtensionMethod ((IMethod)Member);
        //        return reducedMethod;
        //    }
        //}
		
		public VSharpInvocationExpression(
			Expression targetResult, IParameterizedMember member,
			IList<Expression> arguments,
			OverloadResolutionErrors overloadResolutionErrors = OverloadResolutionErrors.None,
			bool isExtensionMethodInvocation = false,
			bool isExpandedForm = false,
			bool isDelegateInvocation = false,
			IList<int> argumentToParameterMap = null,
			IList<Expression> initializerStatements = null,
			IType returnTypeOverride = null
		)
			: base(targetResult, member, arguments, initializerStatements, returnTypeOverride)
		{
			this.OverloadResolutionErrors = overloadResolutionErrors;
			this.IsExtensionMethodInvocation = isExtensionMethodInvocation;
			this.IsExpandedForm = isExpandedForm;
			this.IsDelegateInvocation = isDelegateInvocation;
			this.argumentToParameterMap = argumentToParameterMap;
		}
		
		public override bool IsError {
			get { return this.OverloadResolutionErrors != OverloadResolutionErrors.None; }
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
		
		public override IList<Expression> GetArgumentsForCall()
		{
			Expression[] results = new Expression[Member.Parameters.Count];
			List<Expression> paramsArguments = IsExpandedForm ? new List<Expression>() : null;
			// map arguments to parameters:
			for (int i = 0; i < Arguments.Count; i++) {
				int mappedTo;
				if (argumentToParameterMap != null)
					mappedTo = argumentToParameterMap[i];
				else
					mappedTo = IsExpandedForm ? Math.Min(i, results.Length - 1) : i;
				
				if (mappedTo >= 0 && mappedTo < results.Length) {
					if (IsExpandedForm && mappedTo == results.Length - 1) {
						paramsArguments.Add(RArguments[i]);
					} else {
                        var narr = RArguments[i] as NamedArgumentExpression;
						if (narr != null)
							results[mappedTo] = narr.Argument;
						else
                            results[mappedTo] = RArguments[i];
					}
				}
			}
			if (IsExpandedForm){
				IType arrayType = Member.Parameters.Last().Type;
				IType int32 = Member.Compilation.FindType(KnownTypeCode.Int32);
				Expression[] sizeArguments = { new ConstantExpression(int32, paramsArguments.Count) };
				results[results.Length - 1] = new ArrayCreateExpression(arrayType, sizeArguments, paramsArguments);
			}
			
			for (int i = 0; i < results.Length; i++) {
				if (results[i] == null) {
					if (Member.Parameters[i].IsOptional) {
						results[i] = new ConstantExpression(Member.Parameters[i].Type, Member.Parameters[i].ConstantValue);
					} else {
						results[i] = ErrorExpression.UnknownError;
					}
				}
			}
			
			return results;
		}
	}
}
