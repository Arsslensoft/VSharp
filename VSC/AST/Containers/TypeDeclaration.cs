using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{

    /// <summary>
    /// Parent type of all generic compatible types
    /// </summary>
    public class TypeDeclaration : TypeContainer
    {
   
        public const string DefaultIndexerName = "Item";
        public TypeDeclaration(PackageContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file) 
            : base(ns,name,l,file)
        {
            if (attr != null)
                this.AddAttributes(attr);
            this.mod_flags = mods;
            this.mod_flags = ModifiersExtensions.Check(allowed, mods, Modifiers.INTERNAL, l,Report);
            ApplyModifiers(mods);
           
            this.Kind = kind;
        }
        public TypeDeclaration(TypeContainer ns, Modifiers mods, Modifiers allowed, MemberName name, VSharpAttributes attr, Location l, TypeKind kind, CompilationSourceFile file)
            : base(ns, name, l,file)
        {
            if (attr != null)
                this.AddAttributes(attr);
            this.mod_flags = mods;
            this.mod_flags =ModifiersExtensions.Check(allowed, mods, ns.Name == "default" ? Modifiers.INTERNAL : Modifiers.PRIVATE, l, Report);
            ApplyModifiers(mods);
            this.Kind = kind;
        }
   
        public ParametersCompiled PrimaryConstructorParameters { get; set; }
        public Arguments PrimaryConstructorBaseArguments { get; set; }
        public Location PrimaryConstructorBaseArgumentsStart { get; set; }
        public ToplevelBlock PrimaryConstructorBlock { get; set; }



        bool CanBeUnified(IType a, IType b)
        {
            for (int i = 0; i < a.TypeParameterCount; i++)
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
                    if (IsStatic && !bt.IsKnownType(KnownTypeCode.Object))
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
            else if (Kind == TypeKind.Enum)
            {
                // only primitive integral types
                ResolvedBaseType = rc.CurrentTypeDefinition.DirectBaseTypes.FirstOrDefault();
                if (ResolvedBaseType != null)
                {
                    if (!ResolvedBaseType.IsKnownType(KnownTypeCode.Byte) && !ResolvedBaseType.IsKnownType(KnownTypeCode.SByte)
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
                else if (Kind == TypeKind.Struct)
                    ResolvedBaseType = KnownTypeReference.ValueType.Resolve(rc.CurrentTypeResolveContext);
                else if (Kind == TypeKind.Enum)
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
        bool CheckCircular(IType globaltarget, IType target, IType baseType, ResolveContext rc, IType parentOfBase = null)
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
                if (!CheckCircular(globaltarget, target, t, rc, baseType))
                    return false;

            return true;
        }
    
          /// <summary>
    /// Resolves type parameters
    /// </summary>
    /// <param name="rc"></param>
        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            ResolvedTypeDefinition = rc.CurrentTypeDefinition;
            if (ResolvedTypeDefinition == null)
                return;

            // base types
            ResolveBaseTypes(rc);

            // resolve type parameters and constraints
            foreach (var tp in typeParameters)
                (tp as UnresolvedTypeParameterSpec).DoResolve(rc);

        
            
        }
    }
    
}
