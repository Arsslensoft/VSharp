using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class ClassDeclaration : ClassOrStructDeclaration
    {  
        /// <summary>
        ///   Modifiers allowed in a class declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.ABSTRACT |
            Modifiers.SEALED |
            Modifiers.STATIC;
        public ClassDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l, CompilationSourceFile file)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Class,file)
        {

        }
    }
}