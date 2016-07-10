using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents the result of a member invocation.
	/// Used for field/property/event access.
	/// Also, <see cref="InvocationResolveResult"/> derives from MemberResolveResult.
	/// </summary>
	public class MemberResolveResult : ResolveResult
	{
		readonly IMember member;
		readonly bool isConstant;
		readonly object constantValue;
		readonly ResolveResult targetResult;
		readonly bool isVirtualCall;
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, IType returnTypeOverride = null)
			: base(returnTypeOverride ?? ComputeType(member))
		{
			this.targetResult = targetResult;
			this.member = member;
			var thisRR = targetResult as ThisResolveResult;
			this.isVirtualCall = member.IsOverridable && !(thisRR != null && thisRR.CausesNonVirtualInvocation);
			
			IField field = member as IField;
			if (field != null) {
				isConstant = field.IsConst;
				if (isConstant)
					constantValue = field.ConstantValue;
			}
		}
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, bool isVirtualCall, IType returnTypeOverride = null)
			: base(returnTypeOverride ?? ComputeType(member))
		{
			this.targetResult = targetResult;
			this.member = member;
			this.isVirtualCall = isVirtualCall;
			IField field = member as IField;
			if (field != null) {
				isConstant = field.IsConst;
				if (isConstant)
					constantValue = field.ConstantValue;
			}
		}
		
		static IType ComputeType(IMember member)
		{
			switch (member.SymbolKind) {
				case SymbolKind.Constructor:
					return member.DeclaringType ?? SpecialTypeSpec.UnknownType;
				case SymbolKind.Field:
					if (((IField)member).IsFixed)
						return new PointerTypeSpec(member.ReturnType);
					break;
			}
			return member.ReturnType;
		}
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, IType returnType, bool isConstant, object constantValue)
			: base(returnType)
		{
			this.targetResult = targetResult;
			this.member = member;
			this.isConstant = isConstant;
			this.constantValue = constantValue;
		}
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, IType returnType, bool isConstant, object constantValue, bool isVirtualCall)
			: base(returnType)
		{
			this.targetResult = targetResult;
			this.member = member;
			this.isConstant = isConstant;
			this.constantValue = constantValue;
			this.isVirtualCall = isVirtualCall;
		}
		
		public ResolveResult TargetResult {
			get { return targetResult; }
		}
		
		/// <summary>
		/// Gets the member.
		/// This property never returns null.
		/// </summary>
		public IMember Member {
			get { return member; }
		}
		
		/// <summary>
		/// Gets whether this MemberResolveResult is a virtual call.
		/// </summary>
		public bool IsVirtualCall {
			get { return isVirtualCall; }
		}
		
		public override bool IsCompileTimeConstant {
			get { return isConstant; }
		}
		
		public override object ConstantValue {
			get { return constantValue; }
		}
		
		public override IEnumerable<ResolveResult> GetChildResults()
		{
			if (targetResult != null)
				return new[] { targetResult };
			else
				return Enumerable.Empty<ResolveResult>();
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, member);
		}
		
		public override DomRegion GetDefinitionRegion()
		{
			return member.Region;
		}
	}

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
}
