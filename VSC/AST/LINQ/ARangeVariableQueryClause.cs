namespace VSC.AST
{
    public abstract class ARangeVariableQueryClause : AQueryClause
    {
     
        protected RangeVariable identifier;

        protected ARangeVariableQueryClause(QueryBlock block, RangeVariable identifier, Expression expr, Location loc)
            : base(block, expr, loc)
        {
            this.identifier = identifier;
        }

        public RangeVariable Identifier
        {
            get
            {
                return identifier;
            }
        }

        public FullNamedExpression IdentifierType { get; set; }
    }
}