using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IEvent (event after type substitution).
	/// </summary>
	public class SpecializedEvent : SpecializedMember, IEvent
	{
		readonly IEvent eventDefinition;
		
		public SpecializedEvent(IEvent eventDefinition, TypeParameterSubstitution substitution)
			: base(eventDefinition)
		{
			this.eventDefinition = eventDefinition;
			AddSubstitution(substitution);
		}
		
		public bool CanAdd {
			get { return eventDefinition.CanAdd; }
		}
		
		public bool CanRemove {
			get { return eventDefinition.CanRemove; }
		}
		
		public bool CanInvoke {
			get { return eventDefinition.CanInvoke; }
		}
		
		IMethod addAccessor, removeAccessor, invokeAccessor;
		
		public IMethod AddAccessor {
			get { return WrapAccessor(ref this.addAccessor, eventDefinition.AddAccessor); }
		}
		
		public IMethod RemoveAccessor {
			get { return WrapAccessor(ref this.removeAccessor, eventDefinition.RemoveAccessor); }
		}
		
		public IMethod InvokeAccessor {
			get { return WrapAccessor(ref this.invokeAccessor, eventDefinition.InvokeAccessor); }
		}
	}
}
