using VSC.TypeSystem;

namespace VSC.AST
{
    public abstract class PropertyBasedMember : InterfaceMemberContainer
    {
        public abstract PropertyMethod AccessorFirst { get; }
        public abstract PropertyMethod AccessorSecond { get; }


        protected PropertyBasedMember(TypeContainer parent, FullNamedExpression type, Modifiers mod,
            Modifiers allowed_mod, MemberName name, VSharpAttributes attrs,SymbolKind sym)
            : base(parent, type, mod, allowed_mod, name, attrs,sym)
        {
        }
        // TODO:IMPL
        protected void CheckAccessorNameConflict(PropertyMethod m)
        {
            
        }
    }
}