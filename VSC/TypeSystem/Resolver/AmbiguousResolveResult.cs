using System;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents an ambiguous type resolve result.
	/// </summary>
	public class AmbiguousTypeResolveResult : TypeResolveResult
	{
		public AmbiguousTypeResolveResult(IType type) : base(type)
		{
		}
		
		public override bool IsError {
			get { return true; }
		}
	}
	
	/// <summary>
	/// Represents an ambiguous field/property/event access.
	/// </summary>
	public class AmbiguousMemberResolveResult : MemberResolveResult
	{
		public AmbiguousMemberResolveResult(ResolveResult targetResult, IMember member) : base(targetResult, member)
		{
		}
		
		public override bool IsError {
			get { return true; }
		}
	}
}
