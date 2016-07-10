using System;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    [Serializable]
    public class ConstantDeclaration : FieldDeclaration
    {
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public ConstantDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : base(parent, type, mods, AllowedModifiers, name, attr)
        {
            mod_flags |= Modifiers.STATIC;
            IsStatic = true;
        }
        public ConstantDeclaration(TypeContainer parent, ITypeReference type, Modifiers mods, MemberName name, VSharpAttributes attr)
            : base(parent, type, mods, AllowedModifiers, name, attr)
        {
            mod_flags |= Modifiers.STATIC;
            IsStatic = true;
        }
        // For declarators
        public ConstantDeclaration(ConstantDeclaration baseconstant, MemberName name)
            : base(baseconstant, name)
        {
            mod_flags |= Modifiers.STATIC;
            IsStatic = true;
        }

       
     
    }
}