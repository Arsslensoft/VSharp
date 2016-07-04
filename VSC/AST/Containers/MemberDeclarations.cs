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
      

       	// <summary>
		//   Modifiers allowed in a class declaration
		// </summary>
		const Modifiers AllowedFieldModifiers =
			Modifiers.NEW |
			Modifiers.PUBLIC |
			Modifiers.PROTECTED |
			Modifiers.INTERNAL |
			Modifiers.PRIVATE |
			Modifiers.STATIC |
			Modifiers.READONLY;

        IConstantValue constantValue;

        protected override void FreezeInternal()
        {
            FreezableHelper.Freeze(constantValue);
            base.FreezeInternal();
        }

          // For declarators
        public FieldDeclaration(FieldDeclaration baseconstant, MemberName name, Modifiers allowed)
            : this(baseconstant.Parent, baseconstant.ReturnType, baseconstant.mod_flags,allowed, baseconstant.member_name,baseconstant.attributes)
        {
            
        }
        public FieldDeclaration(FieldDeclaration baseconstant, MemberName name)
            : this(baseconstant.Parent, baseconstant.ReturnType, baseconstant.mod_flags, AllowedFieldModifiers, baseconstant.member_name, baseconstant.attributes)
        {

        }


        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : this(parent, type, mods, AllowedFieldModifiers, name, attr)
        {

        }

        public FieldDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mods, allowed, Modifiers.PRIVATE, name);
            if (attr != null)
                foreach (var a in attr.Attrs)
                    this.attributes.Add(a);


            this.returnType = type as ITypeReference;
 

        }
              public FieldDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mods, allowed, Modifiers.PRIVATE, name);

            if (attr != null)
                foreach (var a in attr.Attrs)
                    this.attributes.Add(a);


            this.returnType = type;


        }
        public FieldDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, Modifiers allowed, MemberName name, IList<IUnresolvedAttribute> attr)
            : this(parent, name.Name)
        {
            Parent = parent;
            CheckModifiersAndSetNames(mods, allowed, Modifiers.PRIVATE, name);

            if (attr != null)
                foreach (var a in attr)
                    this.attributes.Add(a);


            this.returnType = type;


        }

        public FieldDeclaration()
        {
            this.SymbolKind = SymbolKind.Field;
        }

        public FieldDeclaration(IUnresolvedTypeDefinition declaringType, string name)
        {
            type_expr = declaringType as FullNamedExpression;
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
         const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public ConstantDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
             : base(parent, type, mods | Modifiers.STATIC, AllowedModifiers, name, attr)
        {
        }
        public ConstantDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : base(parent, type, mods | Modifiers.STATIC, AllowedModifiers, name, attr)
        {
        }
        // For declarators
        public ConstantDeclaration(ConstantDeclaration baseconstant, MemberName name)
            : base(baseconstant, name)
        {
 
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

                // FIXME: How is `ExplicitThis' used in V#?

                return cc;
            }
        }
        public bool IsInterfaceMethod= false;

        public MethodOrOperator(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,Modifiers allowed,
                   MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            : this(parent, name.Name)
        {
            type_expr = returnType;
            Parent = parent;
            CheckModifiersAndSetNames(mod, allowed, Modifiers.PRIVATE, name);

            if (attrs != null)
                foreach (var a in attrs.Attrs)
                    this.attributes.Add(a);


            this.returnType = returnType as ITypeReference;
            aparameters = parameters;

            this.parameters = parameters.parameters;
            if (member_name.ExplicitInterface != null)
                ApplyExplicit(parameters.parameters);
          
        }


        public MethodOrOperator(TypeContainer parent, ITypeReference returnType, Modifiers mod,Modifiers allowed,
                MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            : this(parent, name.Name)
        {
            type_expr = returnType as FullNamedExpression;
            Parent = parent;
            CheckModifiersAndSetNames(mod, allowed, Modifiers.PRIVATE, name);

            if (attrs != null)
                foreach (var a in attrs.Attrs)
                    this.attributes.Add(a);


            this.returnType = returnType;
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
    [Serializable]
    public class MethodDeclaration : MethodOrOperator
    {
        public MethodDeclaration(TypeContainer parent, FullNamedExpression returnType, Modifiers mod,
                   MemberName name, ParametersCompiled parameters, VSharpAttributes attrs)
            :base(parent,returnType, mod,parent is InterfaceDeclaration ? AllowedModifiersInterface :
				parent is StructDeclaration ? AllowedModifiersStruct :
				AllowedModifiersClass, name, parameters, attrs)
        {

        }

    }

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
    [Serializable]
    public class PropertyDeclaration : PropertyOrIndexer
    {

       Expression expr;
        public Expression Initializer
        {
            get
            {
                return expr;
            }
            set
            {
                expr = value;
            }
        }
        public PropertyDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod,
				 MemberName name, VSharpAttributes attrs)
            : base(parent, type, mod, parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration? AllowedModifiersStruct :
                AllowedModifiersClass, name, attrs)
        {

        }
    }
    public class PropertyMethod : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public PropertyMethod(PropertyOrIndexer method, Modifiers modifiers, MemberName name,ParametersCompiled parameters,VSharpAttributes attrs)
            : base(method.Parent, method.ReturnType, modifiers,AllowedModifiers, name,parameters, attrs )
        {
       
        }
        public PropertyMethod(PropertyOrIndexer method, ITypeReference returnType,Modifiers modifiers, MemberName name,ParametersCompiled parameters, VSharpAttributes attrs)
            : base(method.Parent, returnType, modifiers, AllowedModifiers, name, parameters, attrs)
        {

        }
    }
    public class GetterDeclaration : PropertyMethod
    {
        public GetterDeclaration(PropertyOrIndexer method, Modifiers modifiers,ParametersCompiled par, VSharpAttributes attrs, Location loc)
            : base(method, modifiers, new MemberName("get_" + method.Name, loc), par, attrs)
        {

        }
        public GetterDeclaration(PropertyOrIndexer method, Modifiers modifiers, VSharpAttributes attrs, Location loc)
				: base (method, modifiers, new MemberName("get_"+method.Name, loc),ParametersCompiled.EmptyReadOnlyParameters , attrs)
			{

			}
    }
    public class SetterDeclaration : PropertyMethod
    {
        public SetterDeclaration(PropertyOrIndexer method, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
				: base (method, KnownTypeReference.Void,modifiers, new MemberName("set_"+method.Name, loc),parameters , attrs)
			{

			}
    }
    [Serializable]
    public class IndexerDeclaration : PropertyOrIndexer
    {
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.VIRTUAL |
            Modifiers.SEALED |
            Modifiers.OVERRIDE |
            Modifiers.EXTERN |
            Modifiers.ABSTRACT;

        const Modifiers AllowedInterfaceModifiers =
            Modifiers.NEW;


        readonly ParametersCompiled parameters;
        public IndexerDeclaration(TypeContainer parent, FullNamedExpression type, MemberName name, Modifiers mod, ParametersCompiled parameters, VSharpAttributes attrs)
            : base(parent, type, mod,
                parent is InterfaceDeclaration ? AllowedInterfaceModifiers : AllowedModifiers,
                name, attrs)
        {
            this.parameters = parameters;
        }


        #region Properties

        AParametersCollection Parameters
        {
            get
            {
                return parameters;
            }
        }

        public ParametersCompiled ParameterInfo
        {
            get
            {
                return parameters;
            }
        }

        #endregion
    }
    public sealed class IndexerGetterDeclaration : GetterDeclaration
    {
        public IndexerGetterDeclaration(PropertyOrIndexer property, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
				: base (property, modifiers,parameters, attrs, loc)
			{
				
			}
    }
    public sealed class IndexerSetterDeclaration : SetterDeclaration
    {
        public IndexerSetterDeclaration(PropertyOrIndexer property, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base(property, modifiers, parameters, attrs, loc)
        {

        }
    }


    public  class OperatorDeclaration : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.EXTERN |
            Modifiers.STATIC;

        public readonly VSC.TypeSystem.Resolver.OperatorType OperatorType;

        public OperatorDeclaration(TypeContainer parent, VSC.TypeSystem.Resolver.OperatorType type, FullNamedExpression ret_type, Modifiers mod_flags, ParametersCompiled parameters,
				 ToplevelBlock block, VSharpAttributes attrs, Location loc)
            : base(parent, ret_type, mod_flags, AllowedModifiers, new MemberName(VSC.TypeSystem.Resolver.VSharpResolver.GetMetadataName(type), loc),parameters, attrs )
		{
			OperatorType = type;
			this.block = block;
		}
    }


    public sealed class ConstructorDeclaration : MethodOrOperator
    {

        // <summary>
        //   Modifiers allowed for a constructor.
        // </summary>
        public const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.STATIC |
            Modifiers.EXTERN |
            Modifiers.PRIVATE;

       public static readonly string ConstructorName = ".ctor";
	    public static readonly string TypeConstructorName = ".cctor";


        public ConstructorInitializer Initializer;
        public ConstructorDeclaration(TypeContainer parent, string name, Modifiers mod, VSharpAttributes attrs, ParametersCompiled args, Location loc)
            : base(parent, KnownTypeReference.Void, mod, AllowedModifiers, new MemberName(name, loc), args, attrs)
        {

		}

    }
    public abstract class ConstructorInitializer : ExpressionStatement
    {
        Arguments argument_list;
        protected ConstructorInitializer(Arguments argument_list, Location loc)
        {
            this.argument_list = argument_list;
            this.loc = loc;
        }

        public Arguments Arguments
        {
            get
            {
                return argument_list;
            }
        }

    }
    public class ConstructorSuperInitializer : ConstructorInitializer
    {
        public ConstructorSuperInitializer(Arguments argument_list, Location l) :
            base(argument_list, l)
        {
        }
    }
    sealed class GeneratedSuperInitializer : ConstructorSuperInitializer
    {
        public GeneratedSuperInitializer(Location loc, Arguments arguments)
            : base(arguments, loc)
        {
        }
    }
    public sealed class ConstructorSelfInitializer : ConstructorInitializer
    {
        public ConstructorSelfInitializer(Arguments argument_list, Location l) :
            base(argument_list, l)
        {
        }
    }


    public sealed class DestructorDeclaration : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
        Modifiers.EXTERN;

        public static readonly string MetadataName = "Finalize";

        public DestructorDeclaration(TypeContainer parent, Modifiers mod, ParametersCompiled parameters, VSharpAttributes attrs, Location l)
			: base (parent,KnownTypeReference.Void, mod,AllowedModifiers,  new MemberName (MetadataName, l), parameters, attrs)
		{
			ModFlags &= ~Modifiers.PRIVATE;
			ModFlags |= Modifiers.PROTECTED | Modifiers.OVERRIDE;
            ApplyModifiers(ModFlags);
		}
    }



    /// <summary>
    /// Default implementation of <see cref="IUnresolvedEvent"/>.
    /// </summary>
    [Serializable]
    public class EventBase : MemberContainer, IUnresolvedEvent
    {

        protected EventBase(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs)
			: this(parent,name.Name)
		{
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
    public sealed class EventDeclaration : EventBase
    {
        public EventDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs)
			: base (parent, type, mod_flags, name, attrs)
		{

		}
    }
    public class EventAccessor : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
         Modifiers.PUBLIC |
         Modifiers.PROTECTED |
         Modifiers.INTERNAL |
         Modifiers.PRIVATE;
        protected readonly EventDeclaration method;
       
        public const string AddPrefix = "add_";
        public const string RemovePrefix = "remove_";
        protected EventAccessor(EventDeclaration method, string prefix,Modifiers mods, VSharpAttributes attrs, Location loc)
            : base(method.Parent, method.ReturnType, mods, AllowedModifiers, new MemberName(prefix + method.Name, loc), ParametersCompiled.CreateImplicitParameter(method.TypeExpression, loc), attrs) 
			{
				this.method = method;
			}
    }
    public sealed class AddEventAccessor : EventAccessor
    {
        public AddEventAccessor(EventDeclaration method,Modifiers mods, VSharpAttributes attrs, Location loc)
            : base(method, AddPrefix, mods, attrs, loc)
			{

			}
    }
    public sealed class RemoveEventAccessor : EventAccessor
    {
        public RemoveEventAccessor(EventDeclaration method, Modifiers mods, VSharpAttributes attrs, Location loc)
				: base (method, RemovePrefix,mods, attrs, loc)
			{
			}
    }



    public class EnumMemberDeclaration : ConstantDeclaration
    {
        public EnumMemberDeclaration(EnumDeclaration parent, MemberName name, VSharpAttributes attrs)
			: base (parent, parent as ITypeReference, Modifiers.PUBLIC, name, attrs)
		{
		}


    }
}
