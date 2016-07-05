using VSC.TypeSystem;

namespace VSC.AST
{
    public class SetterDeclaration : PropertyMethod
    {
        public SetterDeclaration(PropertyOrIndexer method, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base (method, KnownTypeReference.Void,modifiers, new MemberName("set_"+method.Name, loc),parameters , attrs)
        {

        }
    }
}