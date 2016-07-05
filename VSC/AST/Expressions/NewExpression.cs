namespace VSC.AST
{
    public class NewExpression : ExpressionStatement
    {
        protected Arguments arguments;

        //
        // During bootstrap, it contains the RequestedType,
        // but if `type' is not null, it *might* contain a NewDelegate
        // (because of field multi-initialization)
        //
        protected Expression RequestedType;


        public NewExpression(Expression requested_type, Arguments arguments, Location l)
        {
            RequestedType = requested_type;
            this.arguments = arguments;
            loc = l;
        }

        #region Properties
        public Arguments Arguments
        {
            get
            {
                return arguments;
            }
        }
        public Expression TypeExpression
        {
            get
            {
                return RequestedType;
            }
        }

        #endregion
    }
}