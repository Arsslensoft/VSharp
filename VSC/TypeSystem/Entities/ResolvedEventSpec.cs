using System;
using System.Collections.Generic;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	public class ResolvedEventSpec : ResolvedMemberSpec, IEvent
	{
		protected new readonly IUnresolvedEvent unresolved;
		IMethod addAccessor;
		IMethod removeAccessor;
		IMethod invokeAccessor;
		
		public ResolvedEventSpec(IUnresolvedEvent unresolved, ITypeResolveContext parentContext)
			: base(unresolved, parentContext)
		{
			this.unresolved = unresolved;
		}
		
		public bool CanAdd {
			get { return unresolved.CanAdd; }
		}
		
		public bool CanRemove {
			get { return unresolved.CanRemove; }
		}
		
		public bool CanInvoke {
			get { return unresolved.CanInvoke; }
		}
		
		public IMethod AddAccessor {
			get { return GetAccessor(ref addAccessor, unresolved.AddAccessor); }
		}
		
		public IMethod RemoveAccessor {
			get { return GetAccessor(ref removeAccessor, unresolved.RemoveAccessor); }
		}
		
		public IMethod InvokeAccessor {
			get { return GetAccessor(ref invokeAccessor, unresolved.InvokeAccessor); }
		}
		
		public override IMember Specialize(TypeParameterSubstitution substitution)
		{
			if (TypeParameterSubstitution.Identity.Equals(substitution)
			    || DeclaringTypeDefinition == null
			    || DeclaringTypeDefinition.TypeParameterCount == 0)
			{
				return this;
			}
			if (substitution.MethodTypeArguments != null && substitution.MethodTypeArguments.Count > 0)
				substitution = new TypeParameterSubstitution(substitution.ClassTypeArguments, EmptyList<IType>.Instance);
			return new SpecializedEvent(this, substitution);
		}
	}
}
