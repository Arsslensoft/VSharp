using VSC.TypeSystem;

namespace VSC.AST
{
    public class EnumMemberDeclaration : ConstantDeclaration
    {
        public EnumMemberDeclaration(EnumDeclaration parent, MemberName name, VSharpAttributes attrs)
            : base (parent, parent as ITypeReference, Modifiers.PUBLIC, name, attrs)
        {
        }


    }
}