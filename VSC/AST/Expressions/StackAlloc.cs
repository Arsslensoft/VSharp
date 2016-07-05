namespace VSC.AST
{
    public class StackAlloc : Expression
    {
      
        Expression texpr;
        Expression count;

        public StackAlloc(Expression type, Expression count, Location l)
        {
            texpr = type;
            this.count = count;
            loc = l;
        }

        public Expression TypeExpression
        {
            get
            {
                return texpr;
            }
        }

        public Expression CountExpression
        {
            get
            {
                return this.count;
            }
        }
    }
}