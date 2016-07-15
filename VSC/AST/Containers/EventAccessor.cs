using VSC.TypeSystem;

namespace VSC.AST
{
    public class EventAccessor : AbstractPropertyEventMethod
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
            : base(method, new TypeExpression(KnownTypeReference.Void, loc), mods, AllowedModifiers, prefix, ParametersCompiled.CreateImplicitParameter(method.TypeExpression, loc), attrs, loc)
        {
            this.method = method;
        }
    }
}