using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class EnumDeclaration : TypeDeclaration
    {
        /// <summary>
        ///   Modifiers allowed in a enum declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public EnumDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, VSharpAttributes attrs, CompilationSourceFile file)
            : base(parent, mod_flags, AllowedModifiers, name, attrs, name.Location, TypeKind.Enum,file)
        {
            mod_flags |= Modifiers.SEALED;
            IsSealed = true;
        }
    }
}