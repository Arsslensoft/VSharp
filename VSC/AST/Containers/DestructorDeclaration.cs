using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class DestructorDeclaration : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
            Modifiers.EXTERN;

        public static readonly string MetadataName = "Finalize";

        public DestructorDeclaration(TypeContainer parent, Modifiers mod, ParametersCompiled parameters, VSharpAttributes attrs, Location l)
            : base (parent,KnownTypeReference.Void, mod,AllowedModifiers,  new MemberName (MetadataName, l), parameters, attrs)
        {
            ModFlags &= ~Modifiers.PRIVATE;
            ModFlags |= Modifiers.PROTECTED | Modifiers.OVERRIDE;
            ApplyModifiers(ModFlags);
        }
    }
}