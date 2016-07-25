using System.Linq;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
   
    public class EnumMemberDeclaration : ConstantDeclaration
    {
        public EnumMemberDeclaration(EnumDeclaration parent, MemberName name, VSharpAttributes attrs)
            : base (parent, parent as ITypeReference, Modifiers.PUBLIC, name, attrs)
        {
        }

        public override Expression Initializer
        {
            get { return init; }
            set
            {
                init = value;
                if (ConstantValue == null && init != null)
                {
                    ConstantValue = init as IConstantValue;
                    if (ConstantValue != null)
                        ConstantValue = new EnumImplicitCastExpression(type_expr, ConstantValue as Expression, init.Location);
                }
                else if (ConstantValue != null)
                {
                    ConstantValue = (ConstantValue as IConstantValue);
                    if (ConstantValue != null)
                        ConstantValue = new EnumImplicitCastExpression(type_expr, ConstantValue as Expression, init.Location);
                }
            }
        }
  
        public override bool DoResolve(ResolveContext rc)
        {

            ResolvedField = Resolve(rc.CurrentTypeResolveContext, SymbolKind, name) as ResolvedFieldSpec;
            ResolvedField.ConstantLocation = Location;
            return true;
        }
    }
}