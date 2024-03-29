using VSC.TypeSystem;

namespace VSC.AST
{
    public abstract class AbstractPropertyEventMethod : MethodCore
    {
        protected readonly string prefix;
        public PropertyBasedMember Property;
        void ConvertAccessor(TypeContainer currentTypeDefinition, Modifiers accessorModifiers, IUnresolvedMember p, bool memberIsExtern)
        {
            SymbolKind = SymbolKind.Accessor;
            AccessorOwner = p;

        }


        protected AbstractPropertyEventMethod(PropertyBasedMember member, FullNamedExpression type, string prefix, VSharpAttributes attrs, Location loc)
            : base(member.Parent,type, Modifiers.NONE, Modifiers.NONE, SetupName(prefix, member, loc), ParametersCompiled.EmptyReadOnlyParameters, attrs,SymbolKind.Accessor)
        {
            Property = member;
            ConvertAccessor(member.Parent, mod_flags, member, (member.ModFlags & Modifiers.EXTERN) != 0);
            this.prefix = prefix;
        }

        protected AbstractPropertyEventMethod(PropertyBasedMember method, FullNamedExpression returnType,
            Modifiers modifiers, Modifiers allowed,string prefix, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base(method.Parent, returnType, modifiers, allowed, SetupName(prefix, method, loc), parameters, attrs, SymbolKind.Accessor)
        {
            Property = method;
            ConvertAccessor(method.Parent, mod_flags, method, (method.ModFlags & Modifiers.EXTERN) != 0);
        }

        static MemberName SetupName(string prefix, InterfaceMemberContainer member, Location loc)
        {
            return new MemberName(member.MemberName.Left, prefix + member.Name, member.MemberName.ExplicitInterface, loc);
        }
    }
}