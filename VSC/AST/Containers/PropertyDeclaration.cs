using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    [Serializable]
    public class PropertyDeclaration : PropertyOrIndexer
    {

        Expression expr;
        public Expression Initializer
        {
            get
            {
                return expr;
            }
            set
            {
                expr = value;
            }
        }
        public PropertyDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod,
            MemberName name, VSharpAttributes attrs)
            : base(parent, type, mod, parent is InterfaceDeclaration ? AllowedModifiersInterface :
                parent is StructDeclaration? AllowedModifiersStruct :
                    AllowedModifiersClass, name, attrs)
        {

        }
    }
}