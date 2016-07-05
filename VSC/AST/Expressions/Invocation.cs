namespace VSC.AST
{
    /// <summary>
    ///   Invocation of methods or delegates.
    /// </summary>
    public class Invocation : ExpressionStatement
    {
        protected Arguments arguments;
        protected Expression expr;
  

        public Invocation(Expression expr, Arguments arguments)
        {
            this.expr = expr;
            this.arguments = arguments;
            if (expr != null)
            {
                loc = expr.Location;
            }
        }

        public Arguments Arguments
        {
            get
            {
                return arguments;
            }
        }

        public Expression Exp
        {
            get
            {
                return expr;
            }
        }
      
    }
}