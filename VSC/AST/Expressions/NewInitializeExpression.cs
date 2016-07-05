namespace VSC.AST
{
    public class NewInitializeExpression : NewExpression
    {
        CollectionOrObjectInitializers initializers;
        public NewInitializeExpression(FullNamedExpression requested_type, Arguments arguments, CollectionOrObjectInitializers initializers, Location l)
            : base(requested_type, arguments, l)
        {
            this.initializers = initializers;
        }

        public CollectionOrObjectInitializers Initializers
        {
            get
            {
                return initializers;
            }
        }

    }
}