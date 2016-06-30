using System;
using System.Diagnostics.Contracts;
using VSC.TypeSystem;

namespace VSC.TypeSystem
{
	public interface IUnresolvedEvent : IUnresolvedMember
	{
		bool CanAdd { get; }
		bool CanRemove { get; }
		bool CanInvoke { get; }
		
		IUnresolvedMethod AddAccessor { get; }
		IUnresolvedMethod RemoveAccessor { get; }
		IUnresolvedMethod InvokeAccessor { get; }
		
		/// <summary>
		/// Resolves the member.
		/// </summary>
		/// <param name="context">
		/// Context for looking up the member. The context must specify the current assembly.
		/// A <see cref="SimpleTypeResolveContext"/> that specifies the current assembly is sufficient.
		/// </param>
		/// <returns>
		/// Returns the resolved member, or <c>null</c> if the member could not be found.
		/// </returns>
		new IEvent Resolve(ITypeResolveContext context);
	}
	
	public interface IEvent : IMember
	{
		bool CanAdd { get; }
		bool CanRemove { get; }
		bool CanInvoke { get; }
		
		IMethod AddAccessor { get; }
		IMethod RemoveAccessor { get; }
		IMethod InvokeAccessor { get; }
	}
}
