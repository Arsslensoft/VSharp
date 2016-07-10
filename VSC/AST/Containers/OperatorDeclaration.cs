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

        public VSC.TypeSystem.Resolver.OperatorType GetMatchingOperator()
        {
            switch (OperatorType)
            {
                case VSC.TypeSystem.Resolver.OperatorType.Equality:
                    return VSC.TypeSystem.Resolver.OperatorType.Inequality;
                case VSC.TypeSystem.Resolver.OperatorType.Inequality:
                    return VSC.TypeSystem.Resolver.OperatorType.Equality;
                case VSC.TypeSystem.Resolver.OperatorType.True:
                    return VSC.TypeSystem.Resolver.OperatorType.False;
                case VSC.TypeSystem.Resolver.OperatorType.False:
                    return VSC.TypeSystem.Resolver.OperatorType.True;
                case VSC.TypeSystem.Resolver.OperatorType.GreaterThan:
                    return VSC.TypeSystem.Resolver.OperatorType.LessThan;
                case VSC.TypeSystem.Resolver.OperatorType.LessThan:
                    return VSC.TypeSystem.Resolver.OperatorType.GreaterThan;
                case VSC.TypeSystem.Resolver.OperatorType.GreaterThanOrEqual:
                    return VSC.TypeSystem.Resolver.OperatorType.LessThanOrEqual;
                case VSC.TypeSystem.Resolver.OperatorType.LessThanOrEqual:
                    return VSC.TypeSystem.Resolver.OperatorType.GreaterThanOrEqual;
                default:
                    return VSC.TypeSystem.Resolver.OperatorType.TOP;
            }
        }
    }
}