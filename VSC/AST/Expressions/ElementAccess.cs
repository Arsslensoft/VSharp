namespace VSC.AST
{
    /// <summary>
    ///   An Element Access expression.
    ///
    ///   During semantic analysis these are transformed into 
    ///   IndexerAccess, ArrayAccess or a PointerArithmetic.
    /// </summary>
    public class ElementAccess : Expression
    {
        public Arguments Arguments;
        public Expression Expr;
        bool conditional_access_receiver;

        public ElementAccess(Expression e, Arguments args, Location loc)
        {
            Expr = e;
            this.loc = loc;
            this.Arguments = args;
        }

        public bool ConditionalAccess { get; set; }

    }
}