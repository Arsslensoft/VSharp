namespace VSC.AST
{
    public class UnboundTypeArguments : TypeArguments
    {
        Location loc;

        public UnboundTypeArguments(int arity, Location loc)
            : base(new FullNamedExpression[arity])
        {
            this.loc = loc;
        }

        public override bool IsEmpty
        {
            get
            {
                return true;
            }
        }

       
    }
}