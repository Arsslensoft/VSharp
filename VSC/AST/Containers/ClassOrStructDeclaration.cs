using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class ClassOrStructDeclaration : TypeDeclaration
    {
        protected bool HasOp = false;
        protected bool HasEqual = false;
        protected bool HasGHC = false;
        protected bool HasEqOp = false;
        public ClassOrStructDeclaration(PackageContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns,mods,allowed, name,attr, l,kind,file)
        {

        }
        public ClassOrStructDeclaration(TypeContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns,mods,allowed, name, attr, l,kind,file)
        {
            
        }

        public virtual void ResolveMembersAndImplementation(ResolveContext rc)
        {
          
      
            // pending implementation
            List<IEntity> pending_implementation = new List<IEntity>();
            if (Kind == TypeKind.Struct || Kind == TypeKind.Class)
            {
           
                foreach (var bt in ResolvedTypeDefinition.DirectBaseTypes)
                {

                    if (bt.Kind != TypeKind.Interface)
                        pending_implementation.AddRange(bt.GetMembers(x => x.IsAbstract));
                    else pending_implementation.AddRange(bt.GetMembers());
                }
            }

            // Resolve nested types
            foreach (var t in TypeContainers)
                t.Resolve(rc);
            // Resolve members
            foreach (var m in TypeMembers)
            {
                // resolve method
                (m as  IResolve).Resolve(rc);

                if (m is OperatorDeclaration)
                {
                    HasOp = true;
                    if (!HasEqOp &&
                        ((m as OperatorDeclaration).OperatorType == OperatorType.Equality ||
                         (m as OperatorDeclaration).OperatorType == OperatorType.Inequality))
                        HasEqOp = true;
                }

                if (m is MethodDeclaration && m.Name == "Equals" && m.IsOverride && (m as MethodDeclaration).Parameters.Count == 1 && (m as MethodDeclaration).ResolvedMethod.Parameters.First().Type.IsKnownType(KnownTypeCode.Object))
                    HasEqual = true;
                if (m is MethodDeclaration && m.Name == "GetHashCode" && m.IsOverride && (m as MethodDeclaration).Parameters.Count == 0)
                    HasGHC = true;
                

          
                // static members check
                if (IsStatic)
                {
                    if (m is OperatorDeclaration)
                    {
                        rc.Report.Error(188, m.Location, "`{0}': Static classes cannot contain user-defined operators", m.GetSignatureForError());
                        continue;
                    }

                    if (m is DestructorDeclaration)
                    {
                        rc.Report.Error(189, m.Location, "`{0}': Static classes cannot contain destructor", GetSignatureForError());
                        continue;
                    }

                    if (m is IndexerDeclaration)
                    {
                        rc.Report.Error(190, m.Location, "`{0}': cannot declare indexers in a static class", m.GetSignatureForError());
                        continue;
                    }

                    if (m.IsStatic || m is TypeContainer)
                        continue;

                    if (m is ConstructorDeclaration)
                    {
                        rc.Report.Error(191, m.Location, "`{0}': Static classes cannot have instance constructors", GetSignatureForError());
                        continue;
                    }

                    rc.Report.Error(192, m.Location, "`{0}': cannot declare instance members in a static class", m.GetSignatureForError());
                }

            
            }

            // warnings
            if (!HasGHC && HasEqual)
                rc.Report.Warning(184, 3, Location,
                    "`{0}' overrides Object.Equals(object) but does not override Object.GetHashCode()", GetSignatureForError());

            if (HasEqOp)
            {
                if (!HasEqual)
                    rc.Report.Warning(185, 2, Location, "`{0}' defines operator == or operator != but does not override Object.Equals(object o)",
                        GetSignatureForError());

                if (!HasGHC)
                    rc.Report.Warning(186, 2, Location, "`{0}' defines operator == or operator != but does not override Object.GetHashCode()",
                        GetSignatureForError());
            }

            if (HasOp)
                CheckPairedOp(rc);
        }
        // Checks that some operators come in pairs:
        //  == and !=
        // > and <
        // >= and <=
        // true and false
        //
        // They are matched based on the return type and the argument types
        void CheckPairedOp(ResolveContext rc)
        {
            bool has_equality_or_inequality = false;
            List<OperatorType> found_matched = new List<OperatorType>();

            for (int i = 0; i < TypeMembers.Count; ++i)
            {
                var o_a = TypeMembers[i] as OperatorDeclaration;
                if (o_a == null)
                    continue;

                var o_type = o_a.OperatorType;
                if (o_type == OperatorType.Equality || o_type == OperatorType.Inequality)
                    has_equality_or_inequality = true;

                if (found_matched.Contains(o_type))
                    continue;

                var matching_type = o_a.GetMatchingOperator();
                if (matching_type == OperatorType.TOP)
                    continue;
                

                bool pair_found = false;
                for (int ii = 0; ii < TypeMembers.Count; ++ii)
                {
                    var o_b = TypeMembers[ii] as OperatorDeclaration;
                    if (o_b == null || o_b.OperatorType != matching_type)
                        continue;

                    if (o_a.ResolvedMethod.ReturnType !=  o_b.ResolvedMethod.ReturnType)
                        continue;

                    if (o_a.ResolvedMethod.Parameters !=  o_b.ResolvedMethod.Parameters)
                        continue;

                    found_matched.Add(matching_type);
                    pair_found = true;
                    break;
                }

                if (!pair_found)
                {
                    rc.Report.Error(187, o_a.Location,
                        "The operator `{0}' requires a matching operator `{1}' to also be defined",
                        o_a.GetSignatureForError(), ResolveContext.GetMetadataName(matching_type));
                }
            }
        }
        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            base.ResolveWithCurrentContext(rc);
            // check primary ctor
            if (PrimaryConstructorParameters != null)
            {

                foreach (Parameter p in PrimaryConstructorParameters.FixedParameters)
                {
                    if (p.Name == MemberName.Name)
                    {
                        rc.Report.Error(156, p.Location, "Primary constructor of type `{0}' has parameter of same name as containing type",
                            GetSignatureForError());
                    }

                    if (typeParameters != null)
                    {
                        for (int i = 0; i < typeParameters.Count; ++i)
                        {
                            var tp = typeParameters[i];
                            if (p.Name == tp.Name)
                            {
                                rc.Report.Error(157, p.Location, "Primary constructor of type `{0}' has parameter of same name as type parameter `{1}'",
                                    GetSignatureForError(), p.Name);
                            }
                        }
                    }
                }
            }
            // Resolve members
            ResolveMembersAndImplementation(rc);

        }
    }
}