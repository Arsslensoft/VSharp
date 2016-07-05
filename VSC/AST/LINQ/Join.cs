namespace VSC.AST
{
    public class Join : SelectMany
    {
        QueryBlock inner_selector, outer_selector;

        public Join(QueryBlock block, RangeVariable lt, Expression inner, QueryBlock outerSelector, QueryBlock innerSelector, Location loc)
            : base(block, lt, inner, loc)
        {
            this.outer_selector = outerSelector;
            this.inner_selector = innerSelector;
        }

        public QueryBlock InnerSelector
        {
            get
            {
                return inner_selector;
            }
        }

        public QueryBlock OuterSelector
        {
            get
            {
                return outer_selector;
            }
        }

  

   

        protected override string MethodName
        {
            get { return "Join"; }
        }

      
    }
}