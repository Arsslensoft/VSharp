using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public class SetterDeclaration : PropertyMethod
    {
        public SetterDeclaration(PropertyOrIndexer method, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base(method, new TypeExpression(KnownTypeReference.Void, loc), modifiers, "set_", parameters, attrs, loc)
        {

        }
        public override void ResolveWithCurrentContext(ResolveContext rc)
        {
            // TODO:Resolve Accessor Block
        }
    }
}