using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class InterfaceDeclaration : TypeDeclaration
    {	
        /// <summary>
        ///   Modifiers allowed in a interface declaration
        /// </summary>
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public InterfaceDeclaration(TypeContainer parent, MemberName name, Modifiers mod, VSharpAttributes attrs, Location l, CompilationSourceFile file)
            : base(parent, mod, AllowedModifiers, name, attrs, l, TypeKind.Interface,file)
        {
            mod_flags |= Modifiers.ABSTRACT;
            IsAbstract = true;
        }
    }
}