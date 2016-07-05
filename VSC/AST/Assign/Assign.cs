namespace VSC.AST
{
    /// <summary>
    ///   The Assign node takes care of assigning the value of source into
    ///   the expression represented by target.
    /// </summary>
    public abstract class Assign : ExpressionStatement
    {
        protected Expression target, source;

        protected Assign(Expression target, Expression source, Location loc)
        {
            this.target = target;
            this.source = source;
            this.loc = loc;
        }

        public Expression Target
        {
            get { return target; }
        }

        public Expression Source
        {
            get
            {
                return source;
            }
        }

    }
}