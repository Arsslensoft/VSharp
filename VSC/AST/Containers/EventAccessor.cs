using VSC.TypeSystem;

namespace VSC.AST
{
    public class EventAccessor : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        protected readonly EventDeclaration method;
       
        public const string AddPrefix = "add_";
        public const string RemovePrefix = "remove_";
        protected EventAccessor(EventDeclaration method, string prefix,Modifiers mods, VSharpAttributes attrs, Location loc)
            : base(method.Parent, method.ReturnType, mods, AllowedModifiers, new MemberName(prefix + method.Name, loc), ParametersCompiled.CreateImplicitParameter(method.TypeExpression, loc), attrs)
        {
            SymbolKind = SymbolKind.Accessor;
            this.method = method;
        }
    }
}