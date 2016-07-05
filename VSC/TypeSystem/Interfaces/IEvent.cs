using System;
using System.Diagnostics.Contracts;
using VSC.TypeSystem;

namespace VSC.TypeSystem
{
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
