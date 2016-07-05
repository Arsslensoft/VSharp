using System;
using System.Collections.Generic;
using System.Linq;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
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

        public void SetTypeParameters(MemberName mn)
        {
            if (mn.TypeParameters != null)
            {
                int idx = 0;
                this.typeParameters = new List<IUnresolvedTypeParameter>();
                    foreach (var tp in mn.TypeParameters.names)
                        this.typeParameters.Add(new UnresolvedTypeParameterSpec(SymbolKind.TypeDefinition, idx++, tp.Name));
                
            }
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

        public TypeContainer(TypeContainer parent, MemberName name, Location l, CompilationSourceFile file)
            : base(parent, name.Name)
        {
            HasExtensionMethods = false;
            SetTypeParameters(name);
            if(parent.Name != "default")
                parent.NestedTypes.Add(this);
            else file.TopLevelTypeDefinitions.Add(this);
         
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
           // Parent = parent.DefaultType;
            usingScope = parent;
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


            ////foreach (var m in TypeMembers)
            ////    m.Resolve(rc);
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
}