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
            Accessibility = Accessibility.Public;
          
        }

  

        public override bool DoResolve(ResolveContext rc)
        {
            if (ConstantValue == null && Initializer != null)
            {
                ConstantValue = (Initializer.BuilConstantValue(false) as Constant);
                if (ConstantValue != null)
                    ConstantValue = (ConstantValue as Constant).ConvertConstantValue(returnType);
            }
            else if (ConstantValue != null)
            {
                ConstantValue = (ConstantValue as Constant);
                if (ConstantValue != null)
                    ConstantValue = (ConstantValue as Constant).ConvertConstantValue(returnType);
            }


            ResolvedField = Resolve(rc.CurrentTypeResolveContext, SymbolKind, name) as ResolvedFieldSpec;

            if (ResolvedField.ConstantValue  != null)
            {
                ResolveResult rr = ResolvedField.constantValue;
                if(rr.IsError)
                    rc.Report.Error(196,Location, "Cannot convert source type `{0}' to target type `{1}'", rr.type.ToString(),rc.CurrentTypeDefinition.EnumUnderlyingType.ToString());

            }
            else if (Initializer != null)
                rc.Report.Error(196, Location, "Cannot convert source type `{0}' to target type `{1}'", Initializer.Type.ToString(), rc.CurrentTypeDefinition.EnumUnderlyingType.ToString());
            else
                rc.Report.Error(196, Location, "Cannot convert value to target type `{0}'", rc.CurrentTypeDefinition.EnumUnderlyingType.ToString());

            return true;
        }
    }
}