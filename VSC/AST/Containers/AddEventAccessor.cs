using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class AddEventAccessor : EventAccessor
    {
        public AddEventAccessor(EventDeclaration method,Modifiers mods, VSharpAttributes attrs, Location loc)
            : base(method, AddPrefix, mods, attrs, loc)
        {

        }
    }
}