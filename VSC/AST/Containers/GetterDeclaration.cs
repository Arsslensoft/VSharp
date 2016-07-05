using VSC.TypeSystem;

namespace VSC.AST
{
    public class GetterDeclaration : PropertyMethod
    {
        public GetterDeclaration(PropertyOrIndexer method, Modifiers modifiers,ParametersCompiled par, VSharpAttributes attrs, Location loc)
            : base(method, modifiers, new MemberName("get_" + method.Name, loc), par, attrs)
        {

        }
        public GetterDeclaration(PropertyOrIndexer method, Modifiers modifiers, VSharpAttributes attrs, Location loc)
            : base (method, modifiers, new MemberName("get_"+method.Name, loc),ParametersCompiled.EmptyReadOnlyParameters , attrs)
        {

        }
    }
}