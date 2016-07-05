using VSC.Context;

namespace VSC.AST
{
    public abstract class AQueryClause : ShimExpression
    {
        protected class QueryExpressionAccess : MemberAccess
        {
            public QueryExpressionAccess(Expression expr, string methodName, Location loc)
                : base(expr, methodName, loc)
            {
            }

            public QueryExpressionAccess(Expression expr, string methodName, TypeArguments typeArguments, Location loc)
                : base(expr, methodName, typeArguments, loc)
            {
            }

            public void Error_TypeDoesNotContainDefinition(string name)
            {
                CompilerContext.report.Error(5, loc, "An implementation of `{0}' query expression pattern could not be found. " +
                                                        "Are you missing `Std.Linq' using directive.",
                    name);
            }
        }
        protected class QueryExpressionInvocation : Invocation
        {
            public QueryExpressionInvocation(QueryExpressionAccess expr, Arguments arguments)
                : base(expr, arguments)
            {
            }
        }

        public AQueryClause next;
        public QueryBlock block;

        protected AQueryClause(QueryBlock block, Expression expr, Location loc)
            : base(expr)
        {
            this.block = block;
            this.loc = loc;
        }

    
        protected abstract string MethodName { get; }

        public AQueryClause Next
        {
            set
            {
                next = value;
            }
        }

        public AQueryClause Tail
        {
            get
            {
                return next == null ? this : next.Tail;
            }
        }
    }
}