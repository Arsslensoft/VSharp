using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class DestructorDeclaration : MethodCore
    {
        const Modifiers AllowedModifiers =
            Modifiers.EXTERN;

        public static readonly string MetadataName = "Finalize";

        public DestructorDeclaration(TypeContainer parent, Modifiers mod, ParametersCompiled parameters, VSharpAttributes attrs, Location l)
            : base(parent, new TypeExpression(KnownTypeReference.Void, l), mod, AllowedModifiers, new MemberName(MetadataName, l), parameters, attrs, SymbolKind.Destructor)
        {
            ModFlags &= ~Modifiers.PRIVATE;
            ModFlags |= Modifiers.PROTECTED | Modifiers.OVERRIDE;
            ApplyModifiers(ModFlags);
        }
    }
}