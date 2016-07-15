using System.Collections.Generic;
using System.Linq;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public abstract class InterfaceMemberContainer : MemberContainer
    {
  
        public bool IsInterfaceMember;
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
        //
        // If true, this is an explicit interface implementation
        //
  
        protected InterfaceMemberContainer(TypeContainer parent, FullNamedExpression type, Modifiers mod, Modifiers allowed_mod, MemberName name, VSharpAttributes attrs, SymbolKind sym)
            : base (parent, type, mod, allowed_mod, Modifiers.PRIVATE, name, attrs,sym)
        {
            IsInterfaceMember = parent.Kind == TypeKind.Interface;
        }
        protected InterfaceMemberContainer(){}

        /// <summary>
        ///   Performs checks for an explicit interface implementation.  First it
        ///   checks whether the `interface_type' is a base inteface implementation.
        ///   Then it checks whether `name' exists in the interface type.
        /// </summary>
        public bool VerifyImplements(ResolveContext rc, IType InterfaceType)
        {
            var ifaces = Parent.ResolvedTypeDefinition.DirectBaseTypes.Where(x => x.Kind == TypeKind.Interface);
            if (ifaces != null)
            {
                foreach (IType t in ifaces)
                {
                    if (t == InterfaceType)
                        return true;

                    var expanded_base = t.DirectBaseTypes;
                    if (expanded_base == null)
                        continue;

                    foreach (var bt in expanded_base)
                    {
                        if (bt == InterfaceType)
                            return true;
                    }
                }
            }

            rc.Report.Error(540, this.Location, "`{0}': containing type does not implement interface `{1}'",
                this.GetSignatureForError(), InterfaceType.ToString());
            return false;
        }
        protected virtual bool CheckOverride(ResolveContext rc, IMember base_member)
        {
            bool ok = true;
            if (!base_member.IsOverridable)
            {
                rc.Report.Error(506, Location,
                    "`{0}': cannot override inherited member `{1}' because it is not marked virtual, abstract or override",
                    GetSignatureForError(), base_member.ToString());
                ok = false;
            }

            // Now we check that the overriden method is not sealed	
            if (base_member.IsSealed)
            {
                rc.Report.Error(239, Location, "`{0}': cannot override inherited member `{1}' because it is sealed",
                    GetSignatureForError(), base_member.ToString());
                ok = false;
            }

            var base_member_type = base_member.ReturnType;
            if (ResolvedMemberType != base_member_type)
            {
                if (this is PropertyOrIndexer)
                {
                    rc.Report.Error(1715, Location, "`{0}': type must be `{1}' to match overridden member `{2}'",
                        GetSignatureForError(), base_member_type.ToString(), base_member.ToString());
                    ok = false;
                }
                else
                {
                    rc.Report.Error(508, Location, "`{0}': return type must be `{1}' to match overridden member `{2}'",
                        GetSignatureForError(), base_member_type.ToString(), base_member.ToString());
                    ok = false;
                }

            }
            return ok;
        }
        protected virtual bool CheckAccessModifiers(IMember this_member, IMember base_member)
        {

            if (base_member.Accessibility == Accessibility.ProtectedOrInternal)
            {
                //
                // It must be at least "protected"
                //
                if (this_member.Accessibility != Accessibility.Protected && this_member.Accessibility != Accessibility.ProtectedOrInternal)
                    return false;


                //
                // when overriding protected internal, the method can be declared
                // protected internal only within the same assembly or assembly
                // which has InternalsVisibleTo
                //
                if (this_member.Accessibility == Accessibility.Internal || this_member.Accessibility == Accessibility.ProtectedOrInternal)
                    return (base_member.DeclaringType as IEntity).IsInternalAccessible(this_member.ParentAssembly);


                //
                // protected overriding protected internal inside same assembly
                // requires internal modifier as well
                //
                if ((base_member.DeclaringType as IEntity).IsInternalAccessible(this_member.ParentAssembly))
                    return false;


                return true;
            }

            return base_member.Accessibility == this_member.Accessibility;
        }
        protected bool CheckParameters(IList<IParameter> parameters, ResolveContext rc)
        {

            bool error = false;
            for (int i = 0; i < parameters.Count; ++i)
            {
                ParameterSpec p = parameters[i] as ParameterSpec;

                if (p.IsOptional && ((ResolvedEntity as IMember).IsExplicitInterfaceImplementation || this is OperatorDeclaration || (this is IndexerDeclaration && parameters.Count == 1)))
                    rc.Report.Warning(1066, 1, Location,
                        "The default value specified for optional parameter `{0}' will never be used",
                        Name);

                if (this.ResolvedEntity.IsAccessibleAs(p.Type))
                    continue;

                IType t = p.Type;

                if (this is IndexerDeclaration)
                    rc.Report.Error(55, Location,
                        "Inconsistent accessibility: parameter type `{0}' is less accessible than indexer `{1}'",
                        t.ToString(), GetSignatureForError());
                else if (this is OperatorDeclaration)
                    rc.Report.Error(57, Location,
                        "Inconsistent accessibility: parameter type `{0}' is less accessible than operator `{1}'",
                        t.ToString(), GetSignatureForError());
                else
                    rc.Report.Error(51, Location,
                        "Inconsistent accessibility: parameter type `{0}' is less accessible than method `{1}'",
                        t.ToString(), GetSignatureForError());
                error = true;
            }
            return !error;
        }
        protected override bool CheckBase(ResolveContext rc)
        {
            if (!base.CheckBase(rc))
                return false;

                CheckForDuplications();

            if (IsExplicitInterfaceImplementation)
                return true;

            // For Std.Object only
            if (Parent.ResolvedBaseType.IsKnownType(KnownTypeCode.Object))
                return true;

            // override checking
            IMember candidate;
            bool overrides = false;
            var base_member = FindBaseMember(rc);
            if (IsOverride)
            {
                if (base_member == null)
                {
                    if (this is MethodDeclaration && ((MethodDeclaration) this).ParameterInfo.IsEmpty &&
                        MemberName.Name == DestructorDeclaration.MetadataName && MemberName.Arity == 0)
                        Report.Error(227, Location, "Do not override `{0}'. Use destructor syntax instead",
                            "object.Finalize()");
                    else
                        Report.Error(228, Location,
                            "`{0}' is marked as an override but no suitable {1} found to override",
                            GetSignatureForError(), SimpleName.GetMemberType(this));

                    return false;
                }



                if (!CheckOverride(rc, base_member))
                    return false;

            }



            if (base_member == null)
            {
                if ((mod_flags & Modifiers.NEW) != 0)
                {
                    if (base_member == null)
                        Report.Warning(229, 4, Location, "The member `{0}' does not hide an inherited member. The new keyword is not required",
                            GetSignatureForError());
                    
                }
            }
            else
            {
                if ((ModFlags & Modifiers.NEW) == 0)
                {
                    ModFlags |= Modifiers.NEW;
                    if (!IsCompilerGenerated)
                    {
                        if (!IsInterfaceMember && base_member.IsOverridable)
                            Report.Warning(230, 2, Location, "`{0}' hides inherited member `{1}'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword",
                                GetSignatureForError(), base_member.ToString());
                        else
                            Report.Warning(231, 2, Location, "`{0}' hides inherited member `{1}'. Use the new keyword if hiding was intended",
                                GetSignatureForError(), base_member.ToString());
                        
                    }
                }

                if (!IsInterfaceMember && base_member.IsAbstract && !IsOverride && !IsStatic)
                {
                    switch (base_member.SymbolKind)
                    {
                        case SymbolKind.Event:
                        case SymbolKind.Indexer:
                        case SymbolKind.Method:
                        case SymbolKind.Property:
                            Report.Error(232, Location, "`{0}' hides inherited abstract member `{1}'",
                                GetSignatureForError(), base_member.ToString());
                            break;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Gets base method and its return type
        /// </summary>
        protected virtual IMember FindBaseMember(ResolveContext rc)
        {
            return (ResolvedEntity as ResolvedMemberSpec).FindBaseMembers();
        }
        protected virtual bool CheckForDuplications()
        {
          var  thisMembers = this.Parent.ResolvedTypeDefinition.GetMembers(m => m.Name == ResolvedEntity.Name && !m.IsExplicitInterfaceImplementation, GetMemberOptions.IgnoreInheritedMembers);
            foreach (var m in thisMembers)
            {
                if(m == ResolvedEntity)    
                    continue;

                if (SignatureComparer.Ordinal.Equals(m, ResolvedEntity as IMember))
                {


                    if (this is ConstructorDeclaration && m.SymbolKind == SymbolKind.Constructor)
                    {
                        Report.Error(223, Location,
                            "Overloaded contructor `{0}' cannot differ on use of parameter modifiers only",
                            GetSignatureForError());
                        return true;
                    }
                    else if(this is MethodDeclaration)
                    {
                        Report.Error(224, Location,
                            "Overloaded method `{0}' cannot differ on use of parameter modifiers only",
                            GetSignatureForError());
                        return true;
                    }
                    else if (this is OperatorDeclaration && m.SymbolKind == SymbolKind.Operator)
                    {
                        Report.Error(225, Location, "Duplicate user-defined conversion in type `{0}'",
                            Parent.GetSignatureForError());

                        return true;
                    }

                    Report.Error(226, Location,
            "A member `{0}' is already defined. Rename this member or use different parameter types",
            GetSignatureForError());
                    return true;
                }
            }


            return false;
        }
        public override bool DoResolve(ResolveContext rc)
        {
            if (IsInterfaceMember)
                mod_flags = Modifiers.PUBLIC | Modifiers.ABSTRACT |
                           Modifiers.VIRTUAL | (ModFlags & (Modifiers.NEW));
            



            if (IsExplicitInterfaceImplementation)
            {
                var InterfaceType = (MemberName.ExplicitInterface as ITypeReference).Resolve(rc.CurrentTypeResolveContext);
                if (InterfaceType == null)
                    return false;

                if ((ModFlags & Modifiers.PARTIAL) != 0)
                    rc.Report.Error(754, Location, "A partial method `{0}' cannot explicitly implement an interface",
                        GetSignatureForError());


                if (InterfaceType.Kind != TypeKind.Interface)
                    rc.Report.Error(538, Location, "The type `{0}' in explicit interface declaration is not an interface",
                        InterfaceType.ToString());
                else
                    VerifyImplements(rc, InterfaceType);
            }

      
            return base.DoResolve(rc);
        }

       
    }
}