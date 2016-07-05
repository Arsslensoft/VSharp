namespace VSC.AST
{
    public abstract class ConstructorInitializer : ExpressionStatement
    {
        Arguments argument_list;
        protected ConstructorInitializer(Arguments argument_list, Location loc)
        {
            this.argument_list = argument_list;
            this.loc = loc;
        }

        public Arguments Arguments
        {
            get
            {
                return argument_list;
            }
        }

    }
}