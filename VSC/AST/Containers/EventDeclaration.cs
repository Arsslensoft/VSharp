using VSC.TypeSystem;

namespace VSC.AST
{
    public class EventDeclaration : EventBase
    {
        public bool IsAutoGenerated = false;
        public EventDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs, bool autogen = false)
            : base (parent, type, mod_flags, name, attrs)
        {
            IsAutoGenerated = autogen;

        }
    }
}