using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    [Serializable]
    public abstract class TypeContainer : TypeDefinitionCore, IAstNode, IResolve
    {
        public ITypeDefinition ResolvedTypeDefinition;
        public IType ResolvedBaseType;


        public VSharpAttributes UnattachedAttributes;
       
        public void SetTypeParameters(MemberName mn)
        {
            this.typeParameters = new List<IUnresolvedTypeParameter>();
            if (mn.TypeParameters != null)
            {
                int idx = 0;
         
                    foreach (var tp in mn.TypeParameters.names)
                        this.typeParameters.Add(new UnresolvedTypeParameterSpec(SymbolKind.TypeDefinition, idx++,tp.Location, tp.Name));
                
            }
        }

        public void ApplyModifiers( VSC.TypeSystem.Modifiers modifiers)
        {
            Accessibility = GetAccessibility(modifiers) ?? (DeclaringTypeDefinition != null ? Accessibility.Private : Accessibility.Internal);
            IsAbstract = (modifiers & VSC.TypeSystem.Modifiers.ABSTRACT) != 0;
            IsSealed = (modifiers & VSC.TypeSystem.Modifiers.SEALED) != 0;
            IsShadowing = (modifiers & VSC.TypeSystem.Modifiers.NEW) != 0;
            IsPartial = (modifiers & VSC.TypeSystem.Modifiers.PARTIAL) != 0;
            IsStatic = (modifiers & VSC.TypeSystem.Modifiers.STATIC) != 0;
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
            foreach (FullNamedExpression texpr in baseTypes)
            {
                if (texpr is TypeNameExpression)
                {
                    var te = texpr as TypeNameExpression;
                    te.lookupMode = NameLookupMode.BaseTypeReference;
                    BaseTypes.Add(te);
                }
                else BaseTypes.Add(texpr as ITypeReference);
            }
            

        }
        public virtual void SetBaseTypes(FullNamedExpression texpr)
        {
        
                if (texpr is TypeNameExpression)
                {
                    var te = texpr as TypeNameExpression;
                    te.lookupMode = NameLookupMode.BaseTypeReference;
                    BaseTypes.Add(te);
                }
                else BaseTypes.Add(texpr as ITypeReference);
            


        }
        public virtual void SetConstraints(List<TypeParameterConstraints> constraints_list)
        {
            var tparams = member_name.TypeParameters;
            if (tparams == null)
            {
                CompilerContext.report.Error(3, Location, "Constraints are not allowed on non-generic declarations");
                return;
            }

            foreach (var c in constraints_list)
            {
                var tp = tparams.Find(c.TypeParameter.Value);
                if (tp == null)
                {
                    CompilerContext.report.Error(4, c.Location, "`{0}': A constraint references nonexistent type parameter `{1}'",
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
                    else if (tc is TypeExpr)
                        tp.Constraints.Add(tc as ITypeReference);
                    

                }
            }

            typeParameters = tparams.names.Cast<IUnresolvedTypeParameter>().ToList();
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
        public override Location Location
        {
            get { return loc; }
        }

        public PackageContainer ParentPackageContainer = null;
        public TypeContainer(TypeContainer parent, MemberName name, Location l, CompilationSourceFile file)
            : base(parent, name.Name)
        {
            HasExtensionMethods = false;
            SetTypeParameters(name);
            if (parent.Name != "default")
                parent.NestedTypes.Add(this);
            else
            {
                parent.ParentPackageContainer.TypeContainers.Add(this);
                file.TopLevelTypeDefinitions.Add(this);
            }


            ParentPackageContainer = parent.ParentPackageContainer;
              UnresolvedFile = file;
            this.usingScope = parent.usingScope;
            this.AddDefaultConstructorIfRequired = true;
            GlobalTypeDefinition = false; 
            loc = l;
            Parent = parent; 
            member_name = name;
        }
        public TypeContainer(PackageContainer parent, MemberName name, Location l, CompilationSourceFile file)
            : base(parent.NamespaceName, name.Name)
        {
            SetTypeParameters(name);
            HasExtensionMethods = false;
            UnresolvedFile = file;
            this.AddDefaultConstructorIfRequired = true;
            GlobalTypeDefinition = false;
            loc = l;
            parent.TypeContainers.Add(this);
            ParentPackageContainer = parent;
            usingScope = parent;
            member_name = name;
            Module = parent.Module;
            file.TopLevelTypeDefinitions.Add(this);
        }


        readonly UsingScope usingScope;

        #region UNRESOLVED

        public bool GlobalTypeDefinition { get; set; }
        public override ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
        {
            return new VSharpTypeResolveContext(parentContext.CurrentAssembly, usingScope.ResolveScope(parentContext.Compilation), parentContext.CurrentTypeDefinition);
        }
        public ResolveResult ResolveTypeDefinition(string name, int typeParameterCount, ResolveContext rc)
        {
            ResolveContext previousResolver = rc;
            try
            {
                ITypeDefinition newTypeDefinition = null;
                if (rc.CurrentTypeDefinition != null)
                {
                 
                    foreach (ITypeDefinition nestedType in rc.CurrentTypeDefinition.NestedTypes)
                    {
                        if (nestedType.Name == name && nestedType.TypeParameterCount == typeParameterCount)
                        {
                            newTypeDefinition = nestedType;
                            break;
                        }
                    }
                }
                else if (rc.CurrentUsingScope != null)
                {
                    newTypeDefinition = rc.CurrentUsingScope.Namespace.GetTypeDefinition(name, typeParameterCount);
                }
                if (newTypeDefinition != null)
                    rc = rc.WithCurrentTypeDefinition(newTypeDefinition);

                // resolve children
                ResolveWithCurrentContext(rc);

                return newTypeDefinition != null ? new TypeResolveResult(newTypeDefinition) : (ResolveResult)ErrorResolveResult.UnknownError;
            }
            finally
            {
                rc = previousResolver;
            }
        }

#endregion


        /// <summary>
        /// Resolves base types
        /// </summary>
        /// <param name="rc"></param>
        public abstract void ResolveWithCurrentContext(ResolveContext rc);
        public virtual bool DoResolve(ResolveContext rc)
        {

            // DoResolve type definition
            ResolveResult rr = ResolveTypeDefinition(name, typeParameters.Count, rc);
            if (rr.IsError)
            {

            }
            return true;
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
            if (Parent != null && Parent.name != "default")
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
}