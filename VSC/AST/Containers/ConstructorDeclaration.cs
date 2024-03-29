using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class ConstructorDeclaration : MethodCore
    {

        // <summary>
        //   Modifiers allowed for a constructor.
        // </summary>
        public const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.STATIC |
            Modifiers.EXTERN |
            Modifiers.PRIVATE;

        public static readonly string ConstructorName = ".ctor";
        public static readonly string TypeConstructorName = ".cctor";


        public ConstructorInitializer Initializer;
        public ConstructorDeclaration(TypeContainer parent, string name, Modifiers mod, VSharpAttributes attrs, ParametersCompiled args, Location loc)
            : base(parent, new TypeExpression(parent, loc), mod, AllowedModifiers, new MemberName(name, loc), args, attrs, SymbolKind.Constructor)
        {
        
        }

    }
}