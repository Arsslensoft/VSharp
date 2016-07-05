using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedEvent"/>.
    /// </summary>
    [Serializable]
    public class EventBase : MemberContainer, IUnresolvedEvent
    {
        protected  VSharpAttributes attribs;
        protected EventBase(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs)
            : this(parent,name.Name)
        {
           attribs = attrs; 
            Parent = parent;
            CheckModifiersAndSetNames(mod_flags, parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration ? AllowedModifiersStruct :
                    AllowedModifiersClass, Modifiers.PRIVATE, name);

            type_expr = type;
            if (attrs != null)
                foreach (var a in attrs.Attrs)
                    this.attributes.Add(a);


            this.returnType = type as ITypeReference;


            if (member_name.ExplicitInterface != null)
                ApplyExplicit(null);
        }

        #region Unresolved
        IUnresolvedMethod addAccessor, removeAccessor, invokeAccessor;

        protected override void FreezeInternal()
        {
            base.FreezeInternal();
            FreezableHelper.Freeze(addAccessor);
            FreezableHelper.Freeze(removeAccessor);
            FreezableHelper.Freeze(invokeAccessor);
        }

        public EventBase()
        {
            this.SymbolKind = SymbolKind.Event;
        }

        public EventBase(IUnresolvedTypeDefinition declaringType, string name)
        {
            this.SymbolKind = SymbolKind.Event;
            this.DeclaringTypeDefinition = declaringType;
            this.Name = name;
            if (declaringType != null)
                this.UnresolvedFile = declaringType.UnresolvedFile;
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