using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class RemoveEventAccessor : EventAccessor
    {
        public RemoveEventAccessor(EventDeclaration method, Modifiers mods, VSharpAttributes attrs, Location loc)
            : base (method, RemovePrefix,mods, attrs, loc)
        {
        }
    }
}