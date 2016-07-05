using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class IndexerSetterDeclaration : SetterDeclaration
    {
        public IndexerSetterDeclaration(PropertyOrIndexer property, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base(property, modifiers, parameters, attrs, loc)
        {

        }
    }
}