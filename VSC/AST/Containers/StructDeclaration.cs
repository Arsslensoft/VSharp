using VSC.TypeSystem;

namespace VSC.AST
{
    public class StructDeclaration : ClassOrStructDeclaration
    {
        // <summary>
        //   Modifiers allowed in a struct declaration
        // </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW       |
            Modifiers.PUBLIC    |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL  |
            Modifiers.PRIVATE;


        public StructDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l, CompilationSourceFile file)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Struct,file)
        {
            mod_flags |= Modifiers.SEALED;
            IsSealed = true;
        }
    }
}