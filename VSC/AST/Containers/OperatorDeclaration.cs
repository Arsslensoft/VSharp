using VSC.TypeSystem;

namespace VSC.AST
{
    public  class OperatorDeclaration : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.EXTERN |
            Modifiers.STATIC;

        public readonly VSC.TypeSystem.Resolver.OperatorType OperatorType;

        public OperatorDeclaration(TypeContainer parent, VSC.TypeSystem.Resolver.OperatorType type, FullNamedExpression ret_type, Modifiers mod_flags, ParametersCompiled parameters,
            ToplevelBlock block, VSharpAttributes attrs, Location loc)
            : base(parent, ret_type, mod_flags, AllowedModifiers, new MemberName(VSC.TypeSystem.Resolver.ResolveContext.GetMetadataName(type), loc),parameters, attrs )
        {
            OperatorType = type;
            this.block = block;
            SymbolKind = SymbolKind.Operator;
        }
    }
}