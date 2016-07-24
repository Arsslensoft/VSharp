using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class IndexerGetterDeclaration : GetterDeclaration
    {
        public IndexerGetterDeclaration(PropertyOrIndexer property, Modifiers modifiers, ParametersCompiled parameters, VSharpAttributes attrs, Location loc)
            : base (property, modifiers,parameters, attrs, loc)
        {
				
        }
    }
}