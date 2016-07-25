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
                        if(!typeParameters.Any(x => x.Name == tp.Name))
                        this.typeParameters.Add(new UnresolvedTypeParameterSpec(SymbolKind.TypeDefinition, idx++,tp.Location, tp.Name));
                        else Report.Error(0, tp.Location,
    "Duplicate type parameter `{0}'", tp.ToString());
                
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
        protected Dictionary<string, EntityCore> defined_names;
        public Dictionary<string, EntityCore> DefinedNames
        {
            get
            {
                return defined_names;
            }
        }
        public PackageContainer ParentPackageContainer = null;
        public TypeContainer(TypeContainer parent, MemberName name, Location l, CompilationSourceFile file)
            : base(parent, name.Name)
        {
            defined_names = new Dictionary<string, EntityCore>();
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
            defined_names = new Dictionary<string, EntityCore>();
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
        public AST.Expression ResolveTypeDefinition(string name, int typeParameterCount, ResolveContext rc)
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

                return newTypeDefinition != null ? new TypeExpression(newTypeDefinition) : (AST.Expression)ErrorExpression.UnknownError;
            }
            finally
            {
                rc = previousResolver;
            }
        }

#endregion

        //
        // Adds the member to defined_names table. It tests for duplications and enclosing name conflicts
        //
        public virtual void AddNameToContainer(EntityCore symbol, string name)
        {
            if (((ModFlags | symbol.ModFlags) & Modifiers.COMPILER_GENERATED) != 0)
                return;

            EntityCore mc;
            if (!defined_names.TryGetValue(name, out mc))
            {
                defined_names.Add(name, symbol);
                return;
            }

            
            InterfaceMemberContainer im = mc as InterfaceMemberContainer;
            if (im != null && im.IsExplicitInterfaceImplementation)
                return;

            if ((mc.ModFlags & Modifiers.PARTIAL) != 0 && (symbol is ClassOrStructDeclaration || symbol is InterfaceDeclaration))
            {
                Report.Error(0, symbol.Location,
                  "Missing partial modifier on declaration of type `{0}'. Another partial declaration of this type exists",
                  symbol.GetSignatureForError());
                return;
            }


                Report.Error(0, symbol.Location,
                    "The type `{0}' already contains a definition for `{1}'",
                    GetSignatureForError(), name);
            

            return;
        }
        /// <summary>
        /// Resolves base types
        /// </summary>
        /// <param name="rc"></param>
        public abstract void ResolveWithCurrentContext(ResolveContext rc);
        public virtual bool DoResolve(ResolveContext rc)
        {

            // DoResolve type definition
            AST.Expression rr = ResolveTypeDefinition(name, typeParameters.Count, rc);
            if (rr.IsError)
            {

            }
            return true;
        }
      
        public virtual void AddMember(MemberContainer member)
        {
            AddNameToContainer(member, member.Name);
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