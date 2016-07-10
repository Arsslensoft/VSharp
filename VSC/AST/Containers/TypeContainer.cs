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
    public class TypeContainer : UnresolvedTypeDefinitionSpec, IAstNode, IResolve
    {
        public ITypeDefinition ResolvedTypeDefinition;
        public IType ResolvedBaseType;


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
        }


        readonly UsingScope usingScope;
		
		

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


        bool CanBeUnified(IType a, IType b)
        {
           for(int i = 0; i < a.TypeParameterCount; i++)
               if ((a.TypeArguments[i] is TypeParameterSpec && b.TypeArguments[i] is TypeParameterSpec) || a.TypeArguments[i] == b.TypeArguments[i])
                   continue;
               
               else return false;

            return true;
        }

      
        void ResolveBaseTypes(ResolveContext rc)
        {
            
                int i = 0;
                List<IType> checked_types = new List<IType>();
          
            if (Kind == TypeKind.Class || Kind == TypeKind.Interface || Kind == TypeKind.Struct)
            {

                foreach (var bt in rc.CurrentTypeDefinition.DirectBaseTypes)
                {

                    // duplicate check
                    if (checked_types.Contains(bt))
                    {
                        rc.Report.Error(158, Location,
                            "Duplicate base class `{0}' for type definition `{1}'", bt.ToString(),
                            ResolvedTypeDefinition.ToString());
                        continue;

                    }
                    checked_types.Add(bt);

                    // type parameter check
                    if (bt is TypeParameterSpec)
                    {
                        rc.Report.Error(193, Location, "`{0}': Cannot derive from type parameter `{1}'",
                            GetSignatureForError(), bt.Name);
                        continue;
                        
                    }
                    // static class derive only from object
                    if(IsStatic && !bt.IsKnownType(KnownTypeCode.Object))
                        rc.Report.Error(194, Location, "Static class `{0}' cannot derive from type `{1}'. Static classes must derive from object",
                        GetSignatureForError(), bt.ToString());

                    // multiple inheritance check
                    if (bt.Kind == TypeKind.Class)
                    {


                        if (ResolvedBaseType != null && ResolvedBaseType != bt)
                            rc.Report.Error(159, Location,
                                "`{0}': Classes cannot have multiple base classes (`{1}' and `{2}')",
                                GetSignatureForError(), bt.ToString(), ResolvedBaseType.ToString());

                        // base class is first check
                        else if (i > 0 && Kind == TypeKind.Class && (!bt.IsKnownType(KnownTypeCode.Object) && !bt.IsKnownType(KnownTypeCode.ValueType)))
                            rc.Report.Error(160, Location,
                                "`{0}': Base class must be specified as first, `{1}' is not a the first base class",
                                GetSignatureForError(), bt.ToString());


                        ResolvedBaseType = bt;
                    }
                    else if (bt.Kind != TypeKind.Interface) // not an interface check
                        rc.Report.Error(161, Location, "Type `{0}' is not an interface", bt.ToString());


                    // if its an interface check the base interfaces
                    if (Kind == TypeKind.Interface && !ResolvedTypeDefinition.IsAccessibleAs(bt))
                        rc.Report.Error(162, Location,
                            "Inconsistent accessibility: base interface `{0}' is less accessible than interface `{1}'",
                            bt.ToString(), GetSignatureForError());

                    // circular dependency check
                    CheckCircular(ResolvedTypeDefinition, ResolvedTypeDefinition, bt, rc);
                    // sealed or static check
                    if ((bt as IEntity).IsSealed)
                          rc.Report.Error(163, Location,
                           "`{0}' is a sealed or a static class.",
                           bt.ToString());

                    // Type parameter unification check
                    if (bt.IsParameterized)
                    {
                        var unify = checked_types.Where(x => (x.IsParameterized && x.FullName == bt.FullName)).FirstOrDefault();
                        if (CanBeUnified(unify, bt))
                            rc.Report.Error(183, Location,
                            "`{0}' cannot implement both `{1}' and `{2}' because they may unify for some type parameter substitutions",
                           GetSignatureForError(), bt.ToString(), unify.ToString());
                    }
                    i++;
                }
                // check class accessibility
                if (Kind == TypeKind.Class && ResolvedBaseType != null && !ResolvedTypeDefinition.IsAccessibleAs(ResolvedBaseType))
                    rc.Report.Error(162, Location,
                        "Inconsistent accessibility: base class `{0}' is less accessible than class `{1}'",
                        ResolvedBaseType.ToString(), ResolvedTypeDefinition.ToString());

                // cannot derive from an attribute
                if (ResolvedBaseType != null && ResolvedBaseType.IsKnownType(KnownTypeCode.Attribute) && ResolvedTypeDefinition.IsParameterized)
                    rc.Report.Error(155, Location,
                                "A generic type cannot derive from `{0}' because it is an attribute class",
                                ResolvedBaseType.ToString());
            }
            else if(Kind == TypeKind.Enum)
            {
                // only primitive integral types
                ResolvedBaseType = rc.CurrentTypeDefinition.DirectBaseTypes.FirstOrDefault();
                if (ResolvedBaseType != null)
                {
                    if(!ResolvedBaseType.IsKnownType(KnownTypeCode.Byte) && !ResolvedBaseType.IsKnownType(KnownTypeCode.SByte)
                        && !ResolvedBaseType.IsKnownType(KnownTypeCode.Int16) && !ResolvedBaseType.IsKnownType(KnownTypeCode.UInt16)
                        && !ResolvedBaseType.IsKnownType(KnownTypeCode.Int32) && !ResolvedBaseType.IsKnownType(KnownTypeCode.UInt32)
                        && !ResolvedBaseType.IsKnownType(KnownTypeCode.Int64) && !ResolvedBaseType.IsKnownType(KnownTypeCode.UInt64))
                        rc.Report.Error(164, Location, "Type `{0}' is not sbyte,byte,short,ushort,int,uint,long,ulong", ResolvedBaseType.ToString());
                }
            }

            if (ResolvedBaseType == null)
            {
                if (Kind == TypeKind.Class)
                    ResolvedBaseType = KnownTypeReference.Object.Resolve(rc.CurrentTypeResolveContext);
                else if(Kind == TypeKind.Struct)
                    ResolvedBaseType = KnownTypeReference.ValueType.Resolve(rc.CurrentTypeResolveContext);
                else if(Kind == TypeKind.Enum)
                    ResolvedBaseType = KnownTypeReference.Enum.Resolve(rc.CurrentTypeResolveContext);
                else if (Kind == TypeKind.Delegate)
                    ResolvedBaseType = KnownTypeReference.MulticastDelegate.Resolve(rc.CurrentTypeResolveContext);
            }

        }
      
            
        
        /*
         there is a circular dependance if
         * A is a parent of B, while B is also a child of A
         * A has a parent B which is circular
        
         * Look for all parent types
         * each parent type will be checked against circular dependance with it's base types
         * each parent type must not depend on the target
          
         * */
        bool CheckCircular(IType globaltarget,IType target,IType baseType, ResolveContext rc,IType parentOfBase = null)
        {
            // the target is a parent of the ancestor
            if (target == baseType)
            {
                if (target.Kind == TypeKind.Class)
                    rc.Report.Error(165, Location,
                         "Circular base class dependency involving `{0}' and `{1}'",
                         target.ToString(), globaltarget.ToString());
                else
                    rc.Report.Error(166, Location,
                     "Inherited interface `{0}' causes a cycle in the interface hierarchy of `{1}'",
                     target.ToString(), globaltarget.ToString());

                return false;
            }
            else if (globaltarget == baseType)
            {
                // the main target  is a parent of the ancestor
                if (target.Kind == TypeKind.Class)
                    rc.Report.Error(165, Location,
                         "Circular base class dependency involving `{0}' and `{1}'",
                         target.ToString(), globaltarget.ToString());
                else
                    rc.Report.Error(166, Location,
                     "Inherited interface `{0}' causes a cycle in the interface hierarchy of `{1}'",
                     target.ToString(), globaltarget.ToString());

                return false;
            }

            // Each parent type will be checked against circular dependance with it's base types
            foreach (var bt in baseType.DirectBaseTypes)
                if (!CheckCircular(globaltarget, baseType, bt, rc, baseType))
                    return false;



            // Each parent type must not depend on the target
            foreach (var t in baseType.DirectBaseTypes)
                if (!CheckCircular(globaltarget,target,t, rc,  baseType))
                 return false;

            return true;
        }
    
        /// <summary>
        /// Resolves base types
        /// </summary>
        /// <param name="rc"></param>
        public virtual void ResolveWithCurrentContext(ResolveContext rc)
        {
            ResolvedTypeDefinition = rc.CurrentTypeDefinition;
            if(ResolvedTypeDefinition == null)
                return;
            
            // base types
            ResolveBaseTypes(rc);
            
         
         }
        public virtual bool Resolve(ResolveContext rc)
        {
            // Resolve type definition
            ResolveResult rr = ResolveTypeDefinition(name, typeParameters.Count, rc);
            if (rr.IsError)
            {

            }
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