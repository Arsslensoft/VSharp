namespace VSC.AST
{
    public class SimpleAssign : Assign
    {
        public SimpleAssign(Expression target, Expression source)
            : this(target, source, target.Location)
        {
        }

        public SimpleAssign(Expression target, Expression source, Location loc)
            : base(target, source, loc)
        {
        }
    }
}