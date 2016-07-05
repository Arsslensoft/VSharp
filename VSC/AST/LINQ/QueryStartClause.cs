using System;

namespace VSC.AST
{
    public class QueryStartClause : ARangeVariableQueryClause
    {
        public QueryStartClause(QueryBlock block, Expression expr, RangeVariable identifier, Location loc)
            : base(block, identifier, expr, loc)
        {
            block.AddRangeVariable(identifier);
        }

        protected override string MethodName
        {
            get { throw new NotSupportedException(); }
        }
    }
}