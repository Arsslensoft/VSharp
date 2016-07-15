using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    // It is used as a base class for all property based members
	// This includes properties, indexers, and events

    /// <summary>
    /// Base class for <see cref="IUnresolvedMember"/> implementations.
    /// </summary>
    [Serializable]
    public abstract class MemberContainer : EntityCore, IUnresolvedMember, IAstNode, IResolve
    {

        public abstract IEntity ResolvedEntity { get; }
        public abstract IType ResolvedMemberType { get; }
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


        protected VSharpAttributes attribs;
         protected MemberContainer(){}
        protected MemberContainer(TypeContainer parent, FullNamedExpression type, Modifiers mod, Modifiers allowed_mod, Modifiers def_mod, MemberName name, VSharpAttributes attrs, SymbolKind sym)
        {
            this.SymbolKind = sym;
			this.Parent = parent;
            this.declaringTypeDefinition = parent;
            this.name = name.Name;
            if (parent != null)
                this.UnresolvedFile = parent.UnresolvedFile;
			this.type_expr = type;
            this.attribs = attrs;
            member_name = name;
            mod_flags = mod;
            mod_flags = ModifiersExtensions.Check(allowed_mod, mod, def_mod, name.Location, Report);
            ApplyModifiers(mod);

            if (attrs != null)
                foreach (var a in attrs.Attrs)
                    this.attributes.Add(a);

            this.returnType = type as ITypeReference;

            if (member_name.ExplicitInterface != null)
                ApplyExplicit(null);
		}

        public IList<ITypeReference> GetParameterTypes(IList<IUnresolvedParameter> parameters)
        {

            if (parameters == null || parameters.Count == 0)
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
                        member_name.Name, member_name.TypeParameters != null ?member_name.TypeParameters.Count : 0, GetParameterTypes(parameters))));
            
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
  

      


        public void AcceptVisitor(IVisitor visitor)
        {
            throw new NotImplementedException();
        }

       public virtual bool DoResolve(ResolveContext rc)
       {

           CheckTypeIndependency(rc);
           CheckTypeDependency(rc);
           return true;
       }
  
        
       protected virtual void CheckTypeDependency(ResolveContext rc)
       { 
           // verify accessibility
           if (!ResolvedEntity.IsAccessibleAs(ResolvedMemberType))
           {
             
               if (this is PropertyDeclaration)
                   rc.Report.Error(206, Location,
                       "Inconsistent accessibility: property type `" +
                       ResolvedMemberType.ToString() + "' is less " +
                       "accessible than property `" + GetSignatureForError() + "'");
               else if (this is IndexerDeclaration)
                   rc.Report.Error(207, Location,
                       "Inconsistent accessibility: indexer return type `" +
                       ResolvedMemberType.ToString() + "' is less " +
                       "accessible than indexer `" + GetSignatureForError() + "'");
               else if (this is MethodCore)
               {
                   if (this is OperatorDeclaration)
                       rc.Report.Error(208, Location,
                           "Inconsistent accessibility: return type `" +
                           ResolvedMemberType.ToString() + "' is less " +
                           "accessible than operator `" + GetSignatureForError() + "'");
                   else
                       rc.Report.Error(209, Location,
                           "Inconsistent accessibility: return type `" +
                           ResolvedMemberType.ToString() + "' is less " +
                           "accessible than method `" + GetSignatureForError() + "'");
               }
               else if (this is EventDeclaration)
               {
                   rc.Report.Error(210, Location,
                       "Inconsistent accessibility: event type `{0}' is less accessible than event `{1}'",
                       ResolvedMemberType.ToString(), GetSignatureForError());
               }
               else
               {
                   rc.Report.Error(211, Location,
                             "Inconsistent accessibility: field type `" +
                             ResolvedMemberType.ToString() + "' is less " +
                             "accessible than field `" + GetSignatureForError() + "'");
               }
           }
       }
       protected virtual void CheckTypeIndependency(ResolveContext rc)
       {
           if ((Parent.ModFlags & Modifiers.SEALED) != 0 &&
               (ModFlags & (Modifiers.VIRTUAL | Modifiers.ABSTRACT)) != 0)
           {
               rc.Report.Error(212, Location, "New virtual member `{0}' is declared in a sealed class `{1}'",
                   GetSignatureForError(), Parent.GetSignatureForError());
           }
       }
       protected virtual bool CheckBase(ResolveContext rc)
       {
           CheckProtected(rc);

           return true;
       }


      
       public override string GetSignatureForDocumentation()
       {
           return Parent.GetSignatureForDocumentation() + "." + MemberName.Basename;
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
        internal new class RareFields : EntityCore.RareFields
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
		
        internal override EntityCore.RareFields WriteRareFields()
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

        #region ResolveScope
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
}
