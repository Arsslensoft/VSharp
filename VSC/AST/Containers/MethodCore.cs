using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedMethod" /> interface.
    /// </summary>
    [Serializable]
    public class MethodCore : InterfaceMemberContainer, IUnresolvedMethod
    {



        protected ParametersCompiled aparameters;
        protected ToplevelBlock block;
        public ToplevelBlock Block
        {
            get
            {
                return block;
            }

            set
            {
                block = value;
            }
        }
        AParametersCollection AParameters
        {
            get { return aparameters; }
        }
        public ParametersCompiled ParameterInfo
        {
            get
            {
                return aparameters;
            }
        }
        public CallingConventions CallingConventions
        {
            get
            {
                CallingConventions cc = aparameters.CallingConvention;
                if (!IsInterfaceMember)
                    if ((ModFlags & Modifiers.STATIC) == 0)
                        cc |= CallingConventions.HasThis;

                // FIXME: How is `ExplicitThis' used in V#?

                return cc;
            }
        }
        public ResolvedMethodSpec ResolvedMethod;


        public override IType ResolvedMemberType
        {
            get { return ResolvedMethod.ReturnType; }
        }
        public override IEntity ResolvedEntity
        {
            get { return ResolvedMethod; }
        }
        public override bool IsOverloadAllowed(MemberContainer overload)
        {
            if (overload is MethodCore)
            {
                caching_flags |= Flags.MethodOverloadsExist;
                return true;
            }

            if (overload is AbstractPropertyEventMethod)
                return true;

            return base.IsOverloadAllowed(overload);
        }


        public MethodCore(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,Modifiers allowed,
            MemberName name, ParametersCompiled parameters, VSharpAttributes attrs, SymbolKind sym)
            : base(parent, returnType, mod, allowed, name, attrs,sym)
        {
 
            this.DeclaringTypeDefinition = parent;
            this.Name = name.Name;
            if (parent != null)
                this.UnresolvedFile = parent.UnresolvedFile;
            this.typeParameters = new List<IUnresolvedTypeParameter>();

            this.aparameters = parameters;
            this.parameters = aparameters.parameters;
          
        }



        public MethodCore()
        {
            
        }



        #region Unresolved
        IList<IUnresolvedAttribute> returnTypeAttributes;
          protected  IList<IUnresolvedTypeParameter> typeParameters;
        IList<IUnresolvedParameter> parameters;
        IUnresolvedMember accessorOwner;

        protected override void FreezeInternal()
        {
            returnTypeAttributes = FreezableHelper.FreezeListAndElements(returnTypeAttributes);
            typeParameters = FreezableHelper.FreezeListAndElements(typeParameters);
            parameters = FreezableHelper.FreezeListAndElements(parameters);
            base.FreezeInternal();
        }

        public virtual void ResolveWithCurrentContext(ResolveContext rc)
        {
            
        }
        public override bool DoResolve(ResolveContext rc)
        {
            ResolveContext oldResolver = rc;
            try
            {
                IMember member = null;

                if (member == null)
                {
                    // Re-discover the method:
                    SymbolKind symbolKind = SymbolKind;
                    var parameterTypes = parameters.Select(p => p.Type).ToList();
                    if (symbolKind == SymbolKind.Constructor)
                    {
                        string name = IsStatic ? ".cctor" : ".ctor";
                        member = Resolve(
                            rc.CurrentTypeResolveContext, symbolKind, name,
                            parameterTypeReferences: parameterTypes);
                    }
                    else if (symbolKind == SymbolKind.Destructor)
                        member = Resolve(rc.CurrentTypeResolveContext, symbolKind, "Finalize");
                    else
                    {
                        string[] typeParameterNames = typeParameters.Select(tp => tp.Name).ToArray();
                        ITypeReference explicitInterfaceType = member_name.ExplicitInterface as ITypeReference;
                        member = Resolve(
                            rc.CurrentTypeResolveContext, symbolKind, Name,
                            explicitInterfaceType, typeParameterNames, parameterTypes);
                    }
                }
                rc = rc.WithCurrentMember(member);
                ResolvedMethod = member as ResolvedMethodSpec;
    
			// Check whether arguments were correct.
                if (!CheckParameters(ResolvedMethod.Parameters, rc))
				return false;

			     base.CheckBase (rc);
		
                base.DoResolve(rc);
                ResolveWithCurrentContext(rc);

              
            }
            finally
            {
                rc = oldResolver;
            }
            return true;
        }

        public override object Clone()
        {
            var copy = (MethodCore)base.Clone();
            if (returnTypeAttributes != null)
                copy.returnTypeAttributes = new List<IUnresolvedAttribute>(returnTypeAttributes);
            if (typeParameters != null)
                copy.typeParameters = new List<IUnresolvedTypeParameter>(typeParameters);
            if (parameters != null)
                copy.parameters = new List<IUnresolvedParameter>(parameters);
            return copy;
        }

        public override void ApplyInterningProvider(InterningProvider provider)
        {
            base.ApplyInterningProvider(provider);
            if (provider != null)
            {
                returnTypeAttributes = provider.InternList(returnTypeAttributes);
                typeParameters = provider.InternList(typeParameters);
                parameters = provider.InternList(parameters);
            }
        }

        public IList<IUnresolvedAttribute> ReturnTypeAttributes
        {
            get
            {
                if (returnTypeAttributes == null)
                    returnTypeAttributes = new List<IUnresolvedAttribute>();
                return returnTypeAttributes;
            }
        }

        public IList<IUnresolvedTypeParameter> TypeParameters
        {
            get
            {
                if (typeParameters == null)
                    typeParameters = new List<IUnresolvedTypeParameter>();
                return typeParameters;
            }
        }

        public bool IsExtensionMethod
        {
            get { return flags[FlagExtensionMethod]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagExtensionMethod] = value;
            }
        }

        public bool IsConstructor
        {
            get { return this.SymbolKind == SymbolKind.Constructor; }
        }

        public bool IsDestructor
        {
            get { return this.SymbolKind == SymbolKind.Destructor; }
        }

        public bool IsOperator
        {
            get { return this.SymbolKind == SymbolKind.Operator; }
        }

        public bool IsSupersede
        {
            get { return (mod_flags & Modifiers.SUPERSEDE) != 0; }
     
        }

        public virtual bool HasBody
        {
            get { return flags[FlagHasBody]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagHasBody] = value;
            }
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

        public IUnresolvedMember AccessorOwner
        {
            get { return accessorOwner; }
            set
            {
                ThrowIfFrozen();
                accessorOwner = value;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder("[");
            b.Append(SymbolKind.ToString());
            b.Append(' ');
            if (DeclaringTypeDefinition != null)
            {
                b.Append(DeclaringTypeDefinition.Name);
                b.Append('.');
            }
            b.Append(Name);
            b.Append('(');
            b.Append(string.Join(", ", this.Parameters));
            b.Append("):");
            b.Append(ReturnType.ToString());
            b.Append(']');
            return b.ToString();
        }

        public override IMember CreateResolved(ITypeResolveContext context)
        {
            return new ResolvedMethodSpec(this, context);
        }

        public override IMember Resolve(ITypeResolveContext context)
        {
            if (accessorOwner != null)
            {
                var owner = accessorOwner.Resolve(context);
                if (owner != null)
                {
                    IProperty p = owner as IProperty;
                    if (p != null)
                    {
                        if (p.CanGet && p.Getter.Name == this.Name)
                            return p.Getter;
                        if (p.CanSet && p.Setter.Name == this.Name)
                            return p.Setter;
                    }
                    IEvent e = owner as IEvent;
                    if (e != null)
                    {
                        if (e.CanAdd && e.AddAccessor.Name == this.Name)
                            return e.AddAccessor;
                        if (e.CanRemove && e.RemoveAccessor.Name == this.Name)
                            return e.RemoveAccessor;
                        if (e.CanInvoke && e.InvokeAccessor.Name == this.Name)
                            return e.InvokeAccessor;
                    }
                }
                return null;
            }

            ITypeReference interfaceTypeReference = null;
            if (this.IsExplicitInterfaceImplementation && this.ExplicitInterfaceImplementations.Count == 1)
                interfaceTypeReference = this.ExplicitInterfaceImplementations[0].DeclaringTypeReference;
            return Resolve(ExtendContextForType(context, this.DeclaringTypeDefinition),
                this.SymbolKind, this.Name, interfaceTypeReference,
                this.TypeParameters.Select(tp => tp.Name).ToList(),
                this.Parameters.Select(p => p.Type).ToList());
        }

        IMethod IUnresolvedMethod.Resolve(ITypeResolveContext context)
        {
            return (IMethod)Resolve(context);
        }

        public static MethodCore CreateDefaultConstructor(IUnresolvedTypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
                throw new ArgumentNullException("typeDefinition");
            DomRegion region = typeDefinition.Region;
            region = new DomRegion(region.FileName, region.BeginLine, region.BeginColumn); // remove endline/endcolumn
            return new ConstructorDeclaration(typeDefinition as TypeContainer, ".ctor",( typeDefinition.IsAbstract ? Modifiers.PROTECTED : Modifiers.PUBLIC) | Modifiers.COMPILER_GENERATED, null, ParametersCompiled.EmptyReadOnlyParameters, Location.Null)
            {
                SymbolKind = SymbolKind.Constructor,
                HasBody = true,
                Region = region,
                BodyRegion = region,
                ReturnType = KnownTypeReference.Void
            };
        }

        static readonly IUnresolvedMethod dummyConstructor = CreateDummyConstructor();

        /// <summary>
        /// Returns a dummy constructor instance:
        /// </summary>
        /// <returns>
        /// A public instance constructor with IsSynthetic=true and no declaring type.
        /// </returns>
        public static IUnresolvedMethod DummyConstructor
        {
            get { return dummyConstructor; }
        }

        static IUnresolvedMethod CreateDummyConstructor()
        {
            var m = new MethodCore
            {
                SymbolKind = SymbolKind.Constructor,
                Name = ".ctor",
                ReturnType = KnownTypeReference.Void
            };
            m.mod_flags |= Modifiers.PUBLIC | Modifiers.COMPILER_GENERATED;
            m.Freeze();
            return m;
        }
        #endregion
    }

    public class MethodOrOperator : MethodCore
    {
        public MethodOrOperator(TypeContainer parent, FullNamedExpression returnType, Modifiers mod, Modifiers allowed,
            MemberName name, ParametersCompiled parameters, VSharpAttributes attrs, SymbolKind sym)
            : base(parent, returnType, mod, allowed, name,parameters, attrs, sym)
        {
        }

        public override bool DoResolve(ResolveContext rc)
        {
            return base.DoResolve(rc);
        }
    }
}