using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IUnresolvedEvent"/>.
	/// </summary>
	[Serializable]
	public class UnresolvedEventSpec : UnresolvedMemberSpec, IUnresolvedEvent
	{
		IUnresolvedMethod addAccessor, removeAccessor, invokeAccessor;
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
			FreezableHelper.Freeze(addAccessor);
			FreezableHelper.Freeze(removeAccessor);
			FreezableHelper.Freeze(invokeAccessor);
		}
		
		public UnresolvedEventSpec()
		{
			this.SymbolKind = SymbolKind.Event;
		}
		
		public UnresolvedEventSpec(IUnresolvedTypeDefinition declaringType, string name)
		{
			this.SymbolKind = SymbolKind.Event;
			this.DeclaringTypeDefinition = declaringType;
			this.Name = name;
			if (declaringType != null)
				this.UnresolvedFile = declaringType.UnresolvedFile;
		}
		
		public bool CanAdd {
			get { return addAccessor != null; }
		}
		
		public bool CanRemove {
			get { return removeAccessor != null; }
		}
		
		public bool CanInvoke {
			get { return invokeAccessor != null; }
		}
		
		public IUnresolvedMethod AddAccessor {
			get { return addAccessor; }
			set {
				ThrowIfFrozen();
				addAccessor = value;
			}
		}
		
		public IUnresolvedMethod RemoveAccessor {
			get { return removeAccessor; }
			set {
				ThrowIfFrozen();
				removeAccessor = value;
			}
		}
		
		public IUnresolvedMethod InvokeAccessor {
			get { return invokeAccessor; }
			set {
				ThrowIfFrozen();
				invokeAccessor = value;
			}
		}
		
		public override IMember CreateResolved(ITypeResolveContext context)
		{
			return new ResolvedEventSpec(this, context);
		}
		
		IEvent IUnresolvedEvent.Resolve(ITypeResolveContext context)
		{
			return (IEvent)Resolve(context);
		}
	}
}
