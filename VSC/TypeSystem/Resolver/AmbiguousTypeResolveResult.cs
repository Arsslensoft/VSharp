using System;

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
}
