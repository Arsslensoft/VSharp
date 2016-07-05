namespace VSC.AST
{
    public class GroupBy : AQueryClause
    {
        Expression element_selector;
        QueryBlock element_block;

        public GroupBy(QueryBlock block, Expression elementSelector, QueryBlock elementBlock, Expression keySelector, Location loc)
            : base(block, keySelector, loc)
        {
            //
            // Optimizes clauses like `group A by A'
            //
            if (!elementSelector.Equals(keySelector))
            {
                this.element_selector = elementSelector;
                this.element_block = elementBlock;
            }
        }

        public Expression SelectorExpression
        {
            get
            {
                return element_selector;
            }
        }

    
        protected override string MethodName
        {
            get { return "GroupBy"; }
        }
    }
}