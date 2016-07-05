namespace VSC.AST
{
    public class MovableArgument : Argument
    {
        public MovableArgument(Argument arg, Location l)
            : this(arg.Expr, arg.ArgType,arg.loc)
        {
        }

        protected MovableArgument(Expression expr, AType modifier, Location l)
            : base(expr, modifier, l)
        {
        }

    }
}