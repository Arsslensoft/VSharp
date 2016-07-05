namespace VSC.AST
{
    public class GroupJoin : Join
    {
        readonly RangeVariable into;

        public GroupJoin(QueryBlock block, RangeVariable lt, Expression inner,
            QueryBlock outerSelector, QueryBlock innerSelector, RangeVariable into, Location loc)
            : base(block, lt, inner, outerSelector, innerSelector, loc)
        {
            this.into = into;
        }
        protected override string MethodName
        {
            get { return "GroupJoin"; }
        }

     
    }
}