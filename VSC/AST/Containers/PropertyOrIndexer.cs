using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedProperty"/>.
    /// </summary>
    [Serializable]
    public class PropertyOrIndexer : MemberContainer, IUnresolvedProperty
    {
        public PropertyOrIndexer(TypeContainer parent, FullNamedExpression type, Modifiers mod, Modifiers allowed_mod,
            MemberName name, VSharpAttributes attrs)
            :this(parent,name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mod, allowed_mod, Modifiers.PRIVATE, name);
            type_expr = type;
            if (attrs != null)
                foreach (var a in attrs.Attrs)
                    this.attributes.Add(a);


            this.returnType = type as ITypeReference;

         
            if (member_name.ExplicitInterface != null)
                ApplyExplicit(null);

        }


        public PropertyMethod AccessorFirst
        {
            get
            {
                return first as PropertyMethod;
            }
        }
        public PropertyMethod AccessorSecond
        {
            get
            {
                return first == getter ? setter as PropertyMethod : getter as PropertyMethod;
            }
        }


        #region Unresolved
        IUnresolvedMethod getter, setter, first;
        IList<IUnresolvedParameter> parameters;

        protected override void FreezeInternal()
        {
            parameters = FreezableHelper.FreezeListAndElements(parameters);
            FreezableHelper.Freeze(getter);
            FreezableHelper.Freeze(setter);
            base.FreezeInternal();
        }

        public override object Clone()
        {
            var copy = (PropertyOrIndexer)base.Clone();
            if (parameters != null)
                copy.parameters = new List<IUnresolvedParameter>(parameters);
            return copy;
        }

        public override void ApplyInterningProvider(InterningProvider provider)
        {
            base.ApplyInterningProvider(provider);
            parameters = provider.InternList(parameters);
        }

        public PropertyOrIndexer()
        {
            this.SymbolKind = SymbolKind.Property;
        }

        public PropertyOrIndexer(IUnresolvedTypeDefinition declaringType, string name)
        {
            this.SymbolKind = SymbolKind.Property;
            this.DeclaringTypeDefinition = declaringType;
            this.Name = name;
            if (declaringType != null)
                this.UnresolvedFile = declaringType.UnresolvedFile;
        }

        public bool IsIndexer
        {
            get { return this.SymbolKind == SymbolKind.Indexer; }
        }

        public IList<IUnresolvedParameter> Parameters
        {
            get
            {
                if (parameters == null)
                    parameters = new List<IUnresolvedParameter>();
                return parameters;
            }
        }

        public bool CanGet
        {
            get { return getter != null; }
        }

        public bool CanSet
        {
            get { return setter != null; }
        }

        public IUnresolvedMethod Getter
        {
            get { return getter; }
            set
            {
                ThrowIfFrozen();
                getter = value;
            }
        }

        public IUnresolvedMethod Setter
        {
            get { return setter; }
            set
            {
                ThrowIfFrozen();
                setter = value;
            }
        }

        public override IMember CreateResolved(ITypeResolveContext context)
        {
            return new ResolvedPropertySpec(this, context);
        }

        public override IMember Resolve(ITypeResolveContext context)
        {
            ITypeReference interfaceTypeReference = null;
            if (this.IsExplicitInterfaceImplementation && this.ExplicitInterfaceImplementations.Count == 1)
                interfaceTypeReference = this.ExplicitInterfaceImplementations[0].DeclaringTypeReference;
            return Resolve(ExtendContextForType(context, this.DeclaringTypeDefinition),
                this.SymbolKind, this.Name, interfaceTypeReference,
                parameterTypeReferences: this.Parameters.Select(p => p.Type).ToList());
        }

        IProperty IUnresolvedProperty.Resolve(ITypeResolveContext context)
        {
            return (IProperty)Resolve(context);
        }

        #endregion
    }
}