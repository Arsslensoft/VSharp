using System.Linq;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class PropertyMethod : AbstractPropertyEventMethod
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;

        public override bool HasBody
        {
            get
            {
                return Block != null;
            }
            set { base.HasBody = value; }
        }


        public PropertyMethod(PropertyBasedMember method, FullNamedExpression returnType, Modifiers modifiers, string prefix, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base(method, returnType, modifiers, AllowedModifiers, prefix, parameters, attrs,loc)
        {
         
        }

        void CreateResolvedMethod(ResolveContext rc)
        {
            ResolveContext oldResolver = rc;
            try{
            ITypeReference explicitInterfaceType = member_name.ExplicitInterface as ITypeReference;
           var  member = Resolve(
                rc.CurrentTypeResolveContext, SymbolKind, Name,
                explicitInterfaceType, null, Parameters.Select(x => x.Type).ToArray());

           rc = rc.WithCurrentMember(member);
           ResolvedMethod = member as ResolvedMethodSpec;

           ResolveWithCurrentContext(rc);   }
            finally
            {
                rc = oldResolver;
            }
        }

        public override bool DoResolve(ResolveContext rc)
        {

            //
            // Check for custom access modifier
            //
            if (Accessibility == TypeSystem.Accessibility.None)
                ModFlags |= Property.ModFlags;
            else
            {
                CheckModifiers(rc,ModFlags);
                ModFlags |= (Property.ModFlags & (~Modifiers.AccessibilityMask));
                ModFlags |= Modifiers.PROPERTY_CUSTOM;

                if (Property.IsInterfaceMember)
                {
                    rc.Report.Error(275, Location, "`{0}': accessibility modifiers may not be used on accessors in an interface",
                        GetSignatureForError());
                }
                else if ((ModFlags & Modifiers.PRIVATE) != 0)
                {
                    if ((Property.ModFlags & Modifiers.ABSTRACT) != 0)
                    {
                        rc.Report.Error(442, Location, "`{0}': abstract properties cannot have private accessors", GetSignatureForError());
                    }

                    ModFlags &= ~Modifiers.VIRTUAL;
                }
            }
            CreateResolvedMethod(rc);
            // basic checks
            CheckAbstractExtern(rc);
            CheckProtected(rc);

            return true;
        }
        public override string GetSignatureForError()
        {
            return Property.GetSignatureForError() + "." + name.Substring(0, 3);
        }
        void CheckModifiers(ResolveContext rc,Modifiers modflags)
        {
            if (!ModifiersExtensions.IsRestrictedModifier(modflags & Modifiers.AccessibilityMask, Property.ModFlags & Modifiers.AccessibilityMask))
            {
                rc.Report.Error(273, Location,
                    "The accessibility modifier of the `{0}' accessor must be more restrictive than the modifier of the property or indexer `{1}'",
                    GetSignatureForError(), Property.GetSignatureForError());
            }
        }
      
    }
}