using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedEvent"/>.
    /// </summary>
    [Serializable]
    public class EventBase : PropertyBasedMember, IUnresolvedEvent
    {
        public ResolvedEventSpec ResolvedEvent;
        public override IEntity ResolvedEntity
        {
            get { return ResolvedEvent; }
        }
        public override IType ResolvedMemberType
        {
            get { return ResolvedEvent.ReturnType; }
        }
        protected  VSharpAttributes attribs;
        protected EventBase(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs)
            : base(parent, type, mod_flags, parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration ? AllowedModifiersStruct :
                    AllowedModifiersClass,name , attrs, SymbolKind.Event)
        {
            this.SymbolKind = SymbolKind.Event;
            this.DeclaringTypeDefinition = parent;
            this.Name = name.Name;
            if (parent != null)
                this.UnresolvedFile = parent.UnresolvedFile;
          
        }

        #region Unresolved
        IUnresolvedMethod addAccessor, removeAccessor, invokeAccessor,first;

        protected override void FreezeInternal()
        {
            base.FreezeInternal();
            FreezableHelper.Freeze(addAccessor);
            FreezableHelper.Freeze(removeAccessor);
            FreezableHelper.Freeze(invokeAccessor);
        }

       
        public bool CanAdd
        {
            get { return addAccessor != null; }
        }

        public bool CanRemove
        {
            get { return removeAccessor != null; }
        }

        public bool CanInvoke
        {
            get { return invokeAccessor != null; }
        }

        public IUnresolvedMethod AddAccessor
        {
            get { return addAccessor; }
            set
            {
                ThrowIfFrozen();
                addAccessor = value;
            }
        }

        public IUnresolvedMethod RemoveAccessor
        {
            get { return removeAccessor; }
            set
            {
                ThrowIfFrozen();
                removeAccessor = value;
            }
        }

        public IUnresolvedMethod InvokeAccessor
        {
            get { return invokeAccessor; }
            set
            {
                ThrowIfFrozen();
                invokeAccessor = value;
            }
        }

        public override PropertyMethod AccessorFirst
        {
            get { return first as PropertyMethod; }
        }

        public override PropertyMethod AccessorSecond
        {
            get { return first == addAccessor ? removeAccessor as PropertyMethod : addAccessor as PropertyMethod; }
        }

        public override IMember CreateResolved(ITypeResolveContext context)
        {
            return new ResolvedEventSpec(this, context);
        }

        IEvent IUnresolvedEvent.Resolve(ITypeResolveContext context)
        {
            return (IEvent)Resolve(context);
        }

        #endregion
    }
}