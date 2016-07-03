using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{

   
    /// <summary>
    /// Base class for <see cref="IUnresolvedMember"/> implementations.
    /// </summary>
    [Serializable]
    public abstract class MemberContainer : UnresolvedEntitySpec, IUnresolvedMember, IAstNode, IResolve
    {


        //
        // Common modifiers allowed in a class declaration
        //
        protected const Modifiers AllowedModifiersClass =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.STATIC |
            Modifiers.VIRTUAL |
            Modifiers.SEALED |
            Modifiers.OVERRIDE |
            Modifiers.ABSTRACT |
            Modifiers.EXTERN;

        //
        // Common modifiers allowed in a struct declaration
        //
        protected const Modifiers AllowedModifiersStruct =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.STATIC |
            Modifiers.OVERRIDE |
            Modifiers.EXTERN;

        //
        // Common modifiers allowed in a interface declaration
        //
        protected const Modifiers AllowedModifiersInterface =
            Modifiers.NEW;

        // <summary>
        //   Checks the object @mod modifiers to be in @allowed.
        //   Returns the new mask.  Side effect: reports any
        //   incorrect attributes. 
        // </summary>
        public static Modifiers Check(Modifiers allowed, Modifiers mod, Modifiers def_access, Location l)
        {
            int invalid_flags = (~(int)allowed) & ((int)mod & ((int)Modifiers.TOP - 1));
            int i;

            if (invalid_flags == 0)
            {
                //
                // If no accessibility bits provided
                // then provide the defaults.
                //
                if ((mod & Modifiers.AccessibilityMask) == 0)
                {
                    mod |= def_access;
                    if (def_access != 0)
                        mod |= Modifiers.DEFAULT_ACCESS_MODIFIER;
                    return mod;
                }

                return mod;
            }

            for (i = 1; i < (int)Modifiers.TOP; i <<= 1)
            {
                if ((i & invalid_flags) == 0)
                    continue;

                Error_InvalidModifier((Modifiers)i, l, CompilerContext.report);
            }

            return allowed & mod;
        }

        static void Error_InvalidModifier(Modifiers mod, Location l, Report Report)
        {
            Report.Error(106, l, "The modifier `{0}' is not valid for this item",
                GetModifierName(mod));
        }
        static public string GetModifierName(Modifiers i)
        {
            string s = "";

            switch (i)
            {
                case Modifiers.NEW:
                    s = "new"; break;
                case Modifiers.PUBLIC:
                    s = "public"; break;
                case Modifiers.PROTECTED:
                    s = "protected"; break;
                case Modifiers.INTERNAL:
                    s = "friend"; break;
                case Modifiers.PRIVATE:
                    s = "private"; break;
                case Modifiers.ABSTRACT:
                    s = "abstract"; break;
                case Modifiers.SEALED:
                    s = "final"; break;
                case Modifiers.STATIC:
                    s = "static"; break;
                case Modifiers.READONLY:
                    s = "readonly"; break;
                case Modifiers.VIRTUAL:
                    s = "virtual"; break;
                case Modifiers.OVERRIDE:
                    s = "override"; break;
                case Modifiers.EXTERN:
                    s = "extern"; break;

            }

            return s;
        }
        public virtual void SetConstraints(List<TypeParameterConstraints> constraints_list)
        {
            var tparams = member_name.TypeParameters;
            if (tparams == null)
            {
                CompilerContext.report.Error(80, Location, "Constraints are not allowed on non-generic declarations");
                return;
            }

            foreach (var c in constraints_list)
            {
                var tp = tparams.Find(c.TypeParameter.Value);
                if (tp == null)
                {
                    CompilerContext.report.Error(699, c.Location, "`{0}': A constraint references nonexistent type parameter `{1}'",
                        GetSignatureForError(), c.TypeParameter.Value);
                    continue;
                }

                // add constraint
                foreach (var tc in c.TypeExpressions)
                {
                    if (tc is SpecialContraintExpr)
                    {
                        var sp = tc as SpecialContraintExpr;
                        if (sp.Constraint == SpecialConstraint.Constructor)
                        {
                            tp.HasDefaultConstructorConstraint = true;
                            continue;
                        }
                        else if (sp.Constraint == SpecialConstraint.Class)
                        {
                            tp.HasReferenceTypeConstraint = true;
                            continue;
                        }
                        else if (sp.Constraint == SpecialConstraint.Struct)
                        {
                            tp.HasValueTypeConstraint = true;
                            continue;
                        }


                    }
                    else if (tc is TypeNameExpression)
                    {
                        (tc as TypeNameExpression).lookupMode = NameLookupMode.BaseTypeReference;
                        tp.Constraints.Add(tc as ITypeReference);
                    }
                }
            }
        }
        public TypeContainer Parent=null;
        public virtual string GetSignatureForError()
        {
            if (Parent != null)
            {
                var parent = Parent.GetSignatureForError();
                if (parent == null)
                    return member_name.GetSignatureForError();

                return parent + "." + member_name.GetSignatureForError();

            }
            else
                return member_name.GetSignatureForError();


        }

        public IList<ITypeReference> GetParameterTypes(IList<IUnresolvedParameter> parameters)
        {
            if (parameters.Count == 0)
                return EmptyList<ITypeReference>.Instance;
            ITypeReference[] types = new ITypeReference[parameters.Count];
            for (int i = 0; i < types.Length; i++)
                types[i] = parameters[i].Type;

            return CompilerContext.InternProvider.InternList(types);
        }
        public void ApplyExplicit(IList<IUnresolvedParameter> parameters)
        {
            Accessibility = Accessibility.None;
            IsExplicitInterfaceImplementation = true;
            ExplicitInterfaceImplementations.Add(
              CompilerContext.InternProvider.Intern(new MemberReferenceSpec(
                    SymbolKind,
                      member_name.ExplicitInterface as ITypeReference,
                        member_name.Name, member_name.TypeParameters.Count, GetParameterTypes(parameters))));
        }
        public void ApplyModifiers( VSC.TypeSystem.Modifiers modifiers)
        {
            // members from interfaces are always Public+Abstract. (NOTE: 'new' modifier is valid in interfaces as well.)
            if (DeclaringTypeDefinition.Kind == TypeKind.Interface)
            {
                Accessibility = Accessibility.Public;
                IsAbstract = true;
                IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
                return;
            }
            Accessibility = GetAccessibility(modifiers) ?? Accessibility.Private;
            IsAbstract = (modifiers & VSC.TypeSystem.Modifiers.ABSTRACT) != 0;
            IsOverride = (modifiers & VSC.TypeSystem.Modifiers.OVERRIDE) != 0;
            IsSealed = (modifiers & VSC.TypeSystem.Modifiers.SEALED) != 0;
            IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
            IsStatic = (modifiers & VSC.TypeSystem.Modifiers.STATIC) != 0;
            IsVirtual = (modifiers & VSC.TypeSystem.Modifiers.VIRTUAL) != 0;
        }
        public Accessibility? GetAccessibility(VSC.TypeSystem.Modifiers modifiers)
        {
            switch (modifiers & VSC.TypeSystem.Modifiers.AccessibilityMask)
            {
                case VSC.TypeSystem.Modifiers.PRIVATE:
                    return Accessibility.Private;
                case VSC.TypeSystem.Modifiers.INTERNAL:
                    return Accessibility.Internal;
                case VSC.TypeSystem.Modifiers.PROTECTED | VSC.TypeSystem.Modifiers.INTERNAL:
                    return Accessibility.ProtectedOrInternal;
                case VSC.TypeSystem.Modifiers.PROTECTED:
                    return Accessibility.Protected;
                case VSC.TypeSystem.Modifiers.PUBLIC:
                    return Accessibility.Public;
                default:
                    return null;
            }
        }

        public void CheckModifiersAndSetNames(Modifiers mods, Modifiers allowed_mods, Modifiers def_mod, MemberName name)
        {
            member_name = name;
            mod_flags = mods;
              mod_flags=   Check(allowed_mods, mods, def_mod, name.Location);
            ApplyModifiers(mods);
        }

        protected FullNamedExpression type_expr;
        public FullNamedExpression TypeExpression
        {
            get
            {
                return type_expr;
            }
            set
            {
                type_expr = value;
            }
        }

        protected MemberName member_name;
        public MemberName MemberName
        {
            get { return member_name; }
        }
        /// <summary>
        ///   Location where this declaration happens
        /// </summary>
        public Location Location
        {
            get { return member_name.Location; }
        }

        string comment = "";
        public string DocComment
        {
            get
            {
                return comment;
            }
            set
            {
                if (value == null)
                    return;

                comment += value;
            }
        }

        public void AcceptVisitor(IVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public object DoResolve(ResolveContext rc)
        {
            throw new NotImplementedException();
        }

        public bool Resolve(ResolveContext rc)
        {
            throw new NotImplementedException();
        }

        #region unresolved
       protected ITypeReference returnType = SpecialTypeSpec.UnknownType;
        IList<IMemberReference> interfaceImplementations;

        public override void ApplyInterningProvider(InterningProvider provider)
        {
            base.ApplyInterningProvider(provider);
            interfaceImplementations = provider.InternList(interfaceImplementations);
        }

        protected override void FreezeInternal()
        {
            base.FreezeInternal();
            interfaceImplementations = FreezableHelper.FreezeList(interfaceImplementations);
        }

        public override object Clone()
        {
            var copy = (MemberContainer)base.Clone();
            if (interfaceImplementations != null)
                copy.interfaceImplementations = new List<IMemberReference>(interfaceImplementations);
            return copy;
        }

        /*
        [Serializable]
        internal new class RareFields : UnresolvedEntitySpec.RareFields
        {
            internal IList<IMemberReference> interfaceImplementations;
			
            public override void ApplyInterningProvider(IInterningProvider provider)
            {
                base.ApplyInterningProvider(provider);
                interfaceImplementations = provider.InternList(interfaceImplementations);
            }
			
            protected internal override void FreezeInternal()
            {
                interfaceImplementations = FreezableHelper.FreezeListAndElements(interfaceImplementations);
                base.FreezeInternal();
            }
			
            override Clone(){}
        }
		
        internal override UnresolvedEntitySpec.RareFields WriteRareFields()
        {
            ThrowIfFrozen();
            if (rareFields == null) rareFields = new RareFields();
            return rareFields;
        }*/

        public ITypeReference ReturnType
        {
            get { return returnType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                ThrowIfFrozen();
                returnType = value;
            }
        }

        public bool IsExplicitInterfaceImplementation
        {
            get { return flags[FlagExplicitInterfaceImplementation]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagExplicitInterfaceImplementation] = value;
            }
        }

        public IList<IMemberReference> ExplicitInterfaceImplementations
        {
            get
            {
                /*
                RareFields rareFields = (RareFields)this.rareFields;
                if (rareFields == null || rareFields.interfaceImplementations == null) {
                    rareFields = (RareFields)WriteRareFields();
                    return rareFields.interfaceImplementations = new List<IMemberReference>();
                }
                return rareFields.interfaceImplementations;
                */
                if (interfaceImplementations == null)
                    interfaceImplementations = new List<IMemberReference>();
                return interfaceImplementations;
            }
        }

        public bool IsVirtual
        {
            get { return flags[FlagVirtual]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagVirtual] = value;
            }
        }

        public bool IsOverride
        {
            get { return flags[FlagOverride]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagOverride] = value;
            }
        }

        public bool IsOverridable
        {
            get
            {
                // override or virtual or abstract but not sealed
                return (flags.Data & (FlagOverride | FlagVirtual | FlagAbstract)) != 0 && !this.IsSealed;
            }
        }

        ITypeReference IMemberReference.DeclaringTypeReference
        {
            get { return this.DeclaringTypeDefinition; }
        }

        #region Resolve
        public abstract IMember CreateResolved(ITypeResolveContext context);

        public virtual IMember Resolve(ITypeResolveContext context)
        {
            ITypeReference interfaceTypeReference = null;
            if (this.IsExplicitInterfaceImplementation && this.ExplicitInterfaceImplementations.Count == 1)
                interfaceTypeReference = this.ExplicitInterfaceImplementations[0].DeclaringTypeReference;
            return Resolve(ExtendContextForType(context, this.DeclaringTypeDefinition), this.SymbolKind, this.Name, interfaceTypeReference);
        }

        ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
        {
            return ((IUnresolvedMember)this).Resolve(context);
        }

        protected static ITypeResolveContext ExtendContextForType(ITypeResolveContext assemblyContext, IUnresolvedTypeDefinition typeDef)
        {
            if (typeDef == null)
                return assemblyContext;
            ITypeResolveContext parentContext;
            if (typeDef.DeclaringTypeDefinition != null)
                parentContext = ExtendContextForType(assemblyContext, typeDef.DeclaringTypeDefinition);
            else
                parentContext = assemblyContext;
            ITypeDefinition resolvedTypeDef = typeDef.Resolve(assemblyContext).GetDefinition();
            return typeDef.CreateResolveContext(parentContext).WithCurrentTypeDefinition(resolvedTypeDef);
        }

        public static IMember Resolve(ITypeResolveContext context,
                                      SymbolKind symbolKind,
                                      string name,
                                      ITypeReference explicitInterfaceTypeReference = null,
                                      IList<string> typeParameterNames = null,
                                      IList<ITypeReference> parameterTypeReferences = null)
        {
            if (context.CurrentTypeDefinition == null)
                return null;
            if (parameterTypeReferences == null)
                parameterTypeReferences = EmptyList<ITypeReference>.Instance;
            if (typeParameterNames == null || typeParameterNames.Count == 0)
            {
                // non-generic member
                // In this case, we can simply resolve the parameter types in the given context
                var parameterTypes = parameterTypeReferences.Resolve(context);
                if (explicitInterfaceTypeReference == null)
                {
                    foreach (IMember member in context.CurrentTypeDefinition.Members)
                    {
                        if (member.IsExplicitInterfaceImplementation)
                            continue;
                        if (IsNonGenericMatch(member, symbolKind, name, parameterTypes))
                            return member;
                    }
                }
                else
                {
                    IType explicitInterfaceType = explicitInterfaceTypeReference.Resolve(context);
                    foreach (IMember member in context.CurrentTypeDefinition.Members)
                    {
                        if (!member.IsExplicitInterfaceImplementation)
                            continue;
                        if (member.ImplementedInterfaceMembers.Count != 1)
                            continue;
                        if (IsNonGenericMatch(member, symbolKind, name, parameterTypes))
                        {
                            if (explicitInterfaceType.Equals(member.ImplementedInterfaceMembers[0].DeclaringType))
                                return member;
                        }
                    }
                }
            }
            else
            {
                // generic member
                // In this case, we must specify the correct context for resolving the parameter types
                foreach (IMethod method in context.CurrentTypeDefinition.Methods)
                {
                    if (method.SymbolKind != symbolKind)
                        continue;
                    if (method.Name != name)
                        continue;
                    if (method.Parameters.Count != parameterTypeReferences.Count)
                        continue;
                    // Compare type parameter count and names:
                    if (!typeParameterNames.SequenceEqual(method.TypeParameters.Select(tp => tp.Name)))
                        continue;
                    // Once we know the type parameter names are fitting, we can resolve the
                    // type references in the context of the method:
                    var contextForMethod = context.WithCurrentMember(method);
                    var parameterTypes = parameterTypeReferences.Resolve(contextForMethod);
                    if (!IsParameterTypeMatch(method, parameterTypes))
                        continue;
                    if (explicitInterfaceTypeReference == null)
                    {
                        if (!method.IsExplicitInterfaceImplementation)
                            return method;
                    }
                    else if (method.IsExplicitInterfaceImplementation && method.ImplementedInterfaceMembers.Count == 1)
                    {
                        IType explicitInterfaceType = explicitInterfaceTypeReference.Resolve(contextForMethod);
                        if (explicitInterfaceType.Equals(method.ImplementedInterfaceMembers[0].DeclaringType))
                            return method;
                    }
                }
            }
            return null;
        }

        static bool IsNonGenericMatch(IMember member, SymbolKind symbolKind, string name, IList<IType> parameterTypes)
        {
            if (member.SymbolKind != symbolKind)
                return false;
            if (member.Name != name)
                return false;
            IMethod method = member as IMethod;
            if (method != null && method.TypeParameters.Count > 0)
                return false;
            return IsParameterTypeMatch(member, parameterTypes);
        }

        static bool IsParameterTypeMatch(IMember member, IList<IType> parameterTypes)
        {
            IParameterizedMember parameterizedMember = member as IParameterizedMember;
            if (parameterizedMember == null)
            {
                return parameterTypes.Count == 0;
            }
            else if (parameterTypes.Count == parameterizedMember.Parameters.Count)
            {
                for (int i = 0; i < parameterTypes.Count; i++)
                {
                    IType type1 = parameterTypes[i];
                    IType type2 = parameterizedMember.Parameters[i].Type;
                    if (!type1.Equals(type2))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #endregion
    }
   
    [Serializable]
    public class TypeContainer : UnresolvedTypeDefinitionSpec, IAstNode, IResolve
    {
        public VSharpAttributes UnattachedAttributes;
        string comment = "";
        public string DocComment
        {
            get
            {
                return comment;
            }
            set
            {
                if (value == null)
                    return;

                comment += value;
            }
        }
        private MemberName member_name;
        public MemberName MemberName
        {
            get { return member_name; }
        }


        public void ApplyModifiers( VSC.TypeSystem.Modifiers modifiers)
        {
            Accessibility = GetAccessibility(modifiers) ?? (DeclaringTypeDefinition != null ? Accessibility.Private : Accessibility.Internal);
            IsAbstract = (modifiers & (VSC.TypeSystem.Modifiers.ABSTRACT | VSC.TypeSystem.Modifiers.STATIC)) != 0;
            IsSealed = (modifiers & (VSC.TypeSystem.Modifiers.SEALED | VSC.TypeSystem.Modifiers.STATIC)) != 0;
            IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
            IsPartial = (modifiers & VSC.TypeSystem.Modifiers.PARTIAL) != 0;
        }
        public Accessibility? GetAccessibility(VSC.TypeSystem.Modifiers modifiers)
        {
            switch (modifiers & VSC.TypeSystem.Modifiers.AccessibilityMask)
            {
                case VSC.TypeSystem.Modifiers.PRIVATE:
                    return Accessibility.Private;
                case VSC.TypeSystem.Modifiers.INTERNAL:
                    return Accessibility.Internal;
                case VSC.TypeSystem.Modifiers.PROTECTED | VSC.TypeSystem.Modifiers.INTERNAL:
                    return Accessibility.ProtectedOrInternal;
                case VSC.TypeSystem.Modifiers.PROTECTED:
                    return Accessibility.Protected;
                case VSC.TypeSystem.Modifiers.PUBLIC:
                    return Accessibility.Public;
                default:
                    return null;
            }
        }
        public virtual void SetBaseTypes(List<FullNamedExpression> baseTypes)
        {
            BaseTypes.AddRange(baseTypes.Cast<ITypeReference>());
        }
        public virtual void SetConstraints(List<TypeParameterConstraints> constraints_list)
        {
            var tparams = member_name.TypeParameters;
            if (tparams == null)
            {
                CompilerContext.report.Error(80, Location, "Constraints are not allowed on non-generic declarations");
                return;
            }

            foreach (var c in constraints_list)
            {
                var tp = tparams.Find(c.TypeParameter.Value);
                if (tp == null)
                {
                    CompilerContext.report.Error(699, c.Location, "`{0}': A constraint references nonexistent type parameter `{1}'",
                        GetSignatureForError(), c.TypeParameter.Value);
                    continue;
                }

                // add constraint
                foreach (var tc in c.TypeExpressions)
                {
                    if (tc is SpecialContraintExpr)
                    {
                         var sp = tc as SpecialContraintExpr;
                  if (sp.Constraint == SpecialConstraint.Constructor)
                    {
                        tp.HasDefaultConstructorConstraint = true;
                        continue;
                    }
                    else if (sp.Constraint == SpecialConstraint.Class)
                    {
                        tp.HasReferenceTypeConstraint = true;
                        continue;
                    }
                    else if (sp.Constraint == SpecialConstraint.Struct)
                    {
                        tp.HasValueTypeConstraint = true;
                        continue;
                    }


                    }
                    else if (tc is TypeNameExpression)
                    {
                        (tc as TypeNameExpression).lookupMode = NameLookupMode.BaseTypeReference;
                        tp.Constraints.Add(tc as ITypeReference);
                    }
                }
            }
        }
        public void AddAttributes(VSharpAttributes attr)
        {
            if (attr != null)
                foreach (var att in attr.Attrs)
                    Attributes.Add(att);


        }
        public void AddAttributes(VSharpAttribute att)
        {

            Attributes.Add(att);


        }
        public TypeContainer Parent = null;
        public void ConvertGlobalAttributes(TypeContainer member, CompilationSourceFile csf)
        {
            foreach (VSharpAttribute att in member.Attributes)
                if (att.ExplicitTarget == "assembly")
                    csf.AddAttributes(att);

        }
        public Location loc;
        /// <summary>
        ///   Location where this declaration happens
        /// </summary>
        public Location Location
        {
            get { return loc; }
        }

        public TypeContainer(TypeContainer parent, MemberName name, Location l)
            : base(parent, name.Name)
        {
            this.usingScope = parent.usingScope;
            this.AddDefaultConstructorIfRequired = true;
            GlobalTypeDefinition = false; 
            loc = l;
            Parent = parent; 
            member_name = name;
        }
        public TypeContainer(NamespaceContainer parent, MemberName name, Location l)
            : base(parent.NamespaceName, name.Name)
        {
            	this.AddDefaultConstructorIfRequired = true;
            GlobalTypeDefinition = false;
            loc = l;
            Parent = parent.DefaultType;
            member_name = name;
        }


        readonly UsingScope usingScope;
		
		

        public bool GlobalTypeDefinition { get; set; }
		public override ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
		{
			return new VSharpTypeResolveContext(parentContext.CurrentAssembly, usingScope.Resolve(parentContext.Compilation), parentContext.CurrentTypeDefinition);
		}
        public virtual bool Resolve(ResolveContext rc)
        {
            foreach (var c in TypeContainers)
                c.Resolve(rc);


            foreach (var m in TypeMembers)
                m.Resolve(rc);
            return true;
        }
        public object DoResolve(ResolveContext rc)
        {
            return this;
        }
        public void AddMember(MemberContainer member)
        {
            Members.Add(member);
            member.ApplyInterningProvider(CompilerContext.InternProvider);
        }
        public IList<TypeContainer> TypeContainers
        {
            get
            {
                return NestedTypes.Cast<TypeContainer>().ToList();
            }
        }
        public IList<MemberContainer> TypeMembers
        {
            get
            {
                return this.Members.Cast<MemberContainer>().ToList();
            }
        }
        // 
        // Returns full member name for error message
        //
        public virtual string GetSignatureForError()
        {
            if (Parent != null)
            {
                var parent = Parent.GetSignatureForError();
                if (parent == null)
                    return member_name.GetSignatureForError();
             
                return parent + "." + member_name.GetSignatureForError();

            }
            else
                return member_name.GetSignatureForError();

            
        }
        public virtual void AcceptVisitor(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

   public class NamespaceContainer : UsingScope, IAstNode, IResolve
   {
       public static NamespaceContainer CreateContainers(NamespaceContainer parent, MemberName mn,Location loc)
       {
           if (mn.Left != null)
           {
               NamespaceContainer left = CreateContainers(parent, mn.Left, loc);
               NamespaceContainer current = new NamespaceContainer(left, mn.Name, loc);
               left.AddChildNamespace(current);
               return current;
           }
           else
               return new NamespaceContainer(parent, mn.Name, loc);
           
       }
       public bool DeclarationFound { get; set; }
       public TypeContainer DefaultType { get; set; }


       public VSharpAttributes UnattachedAttributes;
       public NamespaceContainer Parent=null;
       public NamespaceContainer(NamespaceContainer parent, string name,Location loc)
           : base(parent, name)
       {
           _containers = new List<TypeContainer>();
           _ncontainers = new List<NamespaceContainer>();
           DefaultType = new TypeContainer(this,new MemberName( "default",loc),loc);
           DefaultType.IsPartial = true;
           DefaultType.IsSealed = true;
           DefaultType.IsStatic = true;
           DefaultType.IsSynthetic = false; DeclarationFound = false;
           _containers.Add(DefaultType);
           Parent = parent;
       }

       public NamespaceContainer()
       {
           _containers = new List<TypeContainer>();
           _ncontainers = new List<NamespaceContainer>();
           DefaultType = new TypeContainer(this, new MemberName("default", Location.Null), Location.Null);
           DefaultType.IsPartial = true;
           DefaultType.IsSealed = true;
           DefaultType.IsStatic = true;
           DefaultType.IsSynthetic = false; DeclarationFound = false;
           _containers.Add(DefaultType);
       }
       // only types
       private List<TypeContainer> _containers;
       public IList<TypeContainer> TypeContainers
       {
           get
           {
               return _containers;
           }
       }


       private List<NamespaceContainer> _ncontainers;
       public IList<NamespaceContainer> NamespaceContainers
       {
           get
           {
               return _ncontainers;
           }
       }
       public void AddChildNamespace(NamespaceContainer nc)
       {
           _ncontainers.Add(nc);
       }

       public void AddImport(Import imp)
       {
           var u = imp.NamespaceExpression.ToTypeReference(CompilerContext.InternProvider);
           if (imp.Alias == null)
               Usings.Add(u as TypeNameExpression);
           else UsingAliases.Add(new KeyValuePair<string, TypeNameExpression>(imp.Alias.Value, u as TypeNameExpression));
       }
       public bool Resolve(ResolveContext rc)
       {
           foreach (var c in _containers)
               c.Resolve(rc);

           foreach (var n in _ncontainers)
              n.Resolve(rc);
           return true;
       }
       public object DoResolve(ResolveContext rc)
       {
           return this;
       }
       public virtual void AcceptVisitor(IVisitor visitor)
       {
           visitor.Visit(this);
       }
   }

  
}
