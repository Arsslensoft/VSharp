using System;
using VSC.Base;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Refers to the object that is currently being initialized.
	/// Used within <see cref="InvocationResolveResult.InitializerStatements"/>.
	/// </summary>
	public class InitializedObjectResolveResult : ResolveResult
	{
		public InitializedObjectResolveResult(IType type) : base(type)
		{
		}
	}
}
