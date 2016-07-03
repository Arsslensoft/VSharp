using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedField"/>.
    /// </summary>
    [Serializable]
    public class FieldDeclaration : MemberContainer, IUnresolvedField
    {
      

       

        IConstantValue constantValue;

        protected override void FreezeInternal()
        {
            FreezableHelper.Freeze(constantValue);
            base.FreezeInternal();
        }

          // For declarators
        public FieldDeclaration(FieldDeclaration baseconstant, MemberName name)
            : this(baseconstant.Parent, baseconstant.ReturnType, baseconstant.mod_flags, baseconstant.member_name,baseconstant.attributes)
        {
            
        }
        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            member_name = name;
            mod_flags = mods;

            if (attr != null)
                foreach (var a in attr.Attrs)
                    this.attributes.Add(a);


            this.returnType = type as ITypeReference;
            ApplyModifiers(mods);

        }

            public FieldDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, MemberName name, IList<IUnresolvedAttribute> attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            member_name = name;
            mod_flags = mods;

            if (attr != null)
                foreach (var a in attr)
                    this.attributes.Add(a);


            this.returnType = type;
            ApplyModifiers(mods);

        }
        public FieldDeclaration()
        {
            this.SymbolKind = SymbolKind.Field;
        }

        public FieldDeclaration(IUnresolvedTypeDefinition declaringType, string name)
        {
            this.SymbolKind = SymbolKind.Field;
            this.DeclaringTypeDefinition = declaringType;
            this.Name = name;
            if (declaringType != null)
                this.UnresolvedFile = declaringType.UnresolvedFile;
        }

        public bool IsConst
        {
            get { return constantValue != null && !IsFixed; }
        }

        public bool IsReadOnly
        {
            get { return flags[FlagFieldIsReadOnly]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagFieldIsReadOnly] = value;
            }
        }

        public bool IsVolatile
        {
            get { return flags[FlagFieldIsVolatile]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagFieldIsVolatile] = value;
            }
        }

        public bool IsFixed
        {
            get { return flags[FlagFieldIsFixedSize]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagFieldIsFixedSize] = value;
            }
        }

        public IConstantValue ConstantValue
        {
            get { return constantValue; }
            set
            {
                ThrowIfFrozen();
                constantValue = value;
            }
        }

        public override IMember CreateResolved(ITypeResolveContext context)
        {
            return new ResolvedFieldSpec(this, context);
        }

        IField IUnresolvedField.Resolve(ITypeResolveContext context)
        {
            return (IField)Resolve(context);
        }
    }

     [Serializable]
    public class ConstantDeclaration : FieldDeclaration
    {
        public ConstantDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            :base(parent, type, mods,name, attr)
        {
            IsStatic = true;
        }
        // For declarators
        public ConstantDeclaration(ConstantDeclaration baseconstant, MemberName name)
            : base(baseconstant, name)
        {
            IsStatic = true;
        }
    }
    /// <summary>
    /// Default implementation of <see cref="IUnresolvedMethod" /> interface.
    /// </summary>
    [Serializable]
    public class MethodOrOperator : MemberContainer, IUnresolvedMethod
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
                if (!IsInterfaceMethod)
                    if ((ModFlags & Modifiers.STATIC) == 0)
                        cc |= CallingConventions.HasThis;

                // FIXME: How is `ExplicitThis' used in C#?

                return cc;
            }
        }
        public bool IsInterfaceMethod= false;

        public MethodOrOperator(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,
                   MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            : this(parent, name.Name)
        {
            Parent = parent;
            member_name = name;
            mod_flags = mod;

            if (attrs != null)
                foreach (var a in attrs.Attrs)
                    this.attributes.Add(a);


            this.returnType = returnType as ITypeReference;
            ApplyModifiers(mod);
            aparameters = parameters;

            this.parameters = parameters.parameters;
            if (member_name.ExplicitInterface != null)
                ApplyExplicit(parameters.parameters);
          
        }

        #region Unresolved
        IList<IUnresolvedAttribute> returnTypeAttributes;
        IList<IUnresolvedTypeParameter> typeParameters;
        IList<IUnresolvedParameter> parameters;
        IUnresolvedMember accessorOwner;

        protected override void FreezeInternal()
        {
            returnTypeAttributes = FreezableHelper.FreezeListAndElements(returnTypeAttributes);
            typeParameters = FreezableHelper.FreezeListAndElements(typeParameters);
            parameters = FreezableHelper.FreezeListAndElements(parameters);
            base.FreezeInternal();
        }

        public override object Clone()
        {
            var copy = (MethodOrOperator)base.Clone();
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

        public MethodOrOperator()
        {
            this.SymbolKind = SymbolKind.Method;
        }

        public MethodOrOperator(IUnresolvedTypeDefinition declaringType, string name)
        {
            this.SymbolKind = SymbolKind.Method;
            this.DeclaringTypeDefinition = declaringType;
            this.Name = name;
            if (declaringType != null)
                this.UnresolvedFile = declaringType.UnresolvedFile;
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
            get { return flags[FlagSupersededMethod]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagSupersededMethod] = value;
            }
        }
        public bool IsAsync
        {
            get { return flags[FlagAsyncMethod]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagAsyncMethod] = value;
            }
        }

        public bool HasBody
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

        public static MethodOrOperator CreateDefaultConstructor(IUnresolvedTypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
                throw new ArgumentNullException("typeDefinition");
            DomRegion region = typeDefinition.Region;
            region = new DomRegion(region.FileName, region.BeginLine, region.BeginColumn); // remove endline/endcolumn
            return new MethodOrOperator(typeDefinition, ".ctor")
            {
                SymbolKind = SymbolKind.Constructor,
                Accessibility = typeDefinition.IsAbstract ? Accessibility.Protected : Accessibility.Public,
                IsSynthetic = true,
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
            var m = new MethodOrOperator
            {
                SymbolKind = SymbolKind.Constructor,
                Name = ".ctor",
                Accessibility = Accessibility.Public,
                IsSynthetic = true,
                ReturnType = KnownTypeReference.Void
            };
            m.Freeze();
            return m;
        }
        #endregion
    }

    public class MethodDeclaration : MethodOrOperator
    {
        public MethodDeclaration(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,
                   MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            :base(parent,returnType, mod, name, parameters, attrs)
        {

        }

    }
}
