using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedProperty"/>.
    /// </summary>
    [Serializable]
    public class PropertyOrIndexer : PropertyBasedMember, IUnresolvedProperty
    {
        public bool AutoImplemented = false;
        public ResolvedPropertySpec ResolvedProperty;
        public override IEntity ResolvedEntity
        {
            get { return ResolvedProperty; }
        }
        public override IType ResolvedMemberType
        {
            get { return ResolvedProperty.ReturnType; }
        }
        public PropertyOrIndexer(TypeContainer parent, FullNamedExpression type, Modifiers mod, Modifiers allowed_mod,
            MemberName name, VSharpAttributes attrs, SymbolKind sym)
         :base(parent,type, mod,allowed_mod, name,attrs,sym)
        {

  
            this.SymbolKind = SymbolKind.Property;
            this.DeclaringTypeDefinition = parent;
            this.Name = name.Name;
            if (parent != null)
                this.UnresolvedFile = parent.UnresolvedFile;
        }


        public override PropertyMethod AccessorFirst
        {
            get
            {
                return first as PropertyMethod;
            }
        }
        public override PropertyMethod AccessorSecond
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
                if (first == null)
                    first = value;

                getter = value;
            }
        }

        public IUnresolvedMethod Setter
        {
            get { return setter; }
            set
            {
                ThrowIfFrozen();
                if (first == null)
                    first = value;

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

        protected override void CheckTypeDependency(ResolveContext rc)
        {
            base.CheckTypeDependency(rc);

            if (rc.IsStaticType(ResolvedMemberType))
                rc.Report.Error(722, Location,
                   "`{0}': static types cannot be used as return types",
                   ResolvedMemberType.ToString());
        }
        protected override void CheckTypeIndependency(ResolveContext rc)
        {
            base.CheckTypeIndependency(rc);
            //
            // Accessors modifiers check
            //
            if (AccessorSecond != null)
            {
                if ((Getter.ModFlags & Modifiers.AccessibilityMask) != 0 && (Setter.ModFlags & Modifiers.AccessibilityMask) != 0)
                {
                    rc.Report.Error(274, Location, "`{0}': Cannot specify accessibility modifiers for both accessors of the property or indexer",
                        GetSignatureForError());
                }
            }
            else if ((ModFlags & Modifiers.OVERRIDE) == 0 &&
              ((Getter == null && (Setter.ModFlags & Modifiers.AccessibilityMask) != 0) || (Setter == null && (Getter.ModFlags & Modifiers.AccessibilityMask) != 0)))
            {
                rc.Report.Error(276, Location,
                          "`{0}': accessibility modifiers on accessors may only be used if the property or indexer has both a get and a set accessor",
                          GetSignatureForError());
            }
        }
        protected override bool CheckOverride(ResolveContext rc, IMember base_member)
        {
         bool ok =   base.CheckOverride(rc, base_member);
            //TODO:Add check override for property
            return ok;


        }

        public virtual void ResolveWithCurrentContext(ResolveContext rc)
        {
            var resolverWithPropertyAsMember = rc;
       
            // resolve getter
            if (Getter != null)
            {
            rc = rc.WithCurrentMember(((IProperty)ResolvedProperty).Getter);
    
            (Getter as PropertyMethod).DoResolve(rc);
            rc = resolverWithPropertyAsMember;
             }


            // resolve setter
            if (Setter != null)
            {
                rc = rc.WithCurrentMember(((IProperty) ResolvedProperty).Setter);
                (Setter as PropertyMethod).DoResolve(rc);
            }
        }
        


        public override bool DoResolve(ResolveContext resolver)
        {
            ResolveContext oldResolver = resolver;
            try
            {
              
             
                    // Re-discover the property:
             
                    ITypeReference explicitInterfaceType = null;
                    if (IsExplicitInterfaceImplementation)
                        explicitInterfaceType = member_name.ExplicitInterface as ITypeReference;
                
                    
                    ResolvedProperty = MemberContainer.Resolve(
                        resolver.CurrentTypeResolveContext, SymbolKind, name,
                        explicitInterfaceType, parameterTypeReferences: null) as ResolvedPropertySpec;
                
                // We need to use the property as current member so that indexer parameters can be resolved correctly.
                    base.DoResolve(resolver);
                    resolver = resolver.WithCurrentMember(ResolvedProperty);
                ResolveWithCurrentContext(resolver);
            }
            finally
            {
                resolver = oldResolver;
            }
            return true;
        }
    }
}