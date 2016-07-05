namespace VSC.AST
{
    public class NamedArgument : MovableArgument
    {
        public readonly string Name;
        public readonly bool CtorArgument = true;

        public NamedArgument(string name, Location loc, Expression expr)
            : this(name, loc, expr, AType.None)
        {
            CtorArgument = false;
        }

        public NamedArgument(string name, Location loc, Expression expr, AType modifier)
            : base(expr, modifier, loc)
        {
            this.Name = name;
  
        }

        public Location Location
        {
            get { return loc; }
        }
    }
}