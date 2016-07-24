using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class GetterDeclaration : PropertyMethod
    {
        public GetterDeclaration(PropertyOrIndexer method, Modifiers modifiers,ParametersCompiled par, VSharpAttributes attrs, Location loc)
            : base(method, method.TypeExpression, modifiers, "get_", par, attrs,loc)
        {

        }
        public GetterDeclaration(PropertyOrIndexer method, Modifiers modifiers, VSharpAttributes attrs, Location loc)
            : base(method,method.TypeExpression, modifiers, "get_", ParametersCompiled.EmptyReadOnlyParameters, attrs, loc)
        {

        }

        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            // TODO:Resolve Accessor Block
        }
    }
}