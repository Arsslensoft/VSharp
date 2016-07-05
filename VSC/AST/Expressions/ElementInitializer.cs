namespace VSC.AST
{
    public class ElementInitializer : Assign
    {
        public readonly string Name;

        public ElementInitializer(string name, Expression initializer, Location loc)
            : base(null, initializer, loc)
        {
            this.Name = name;
        }
    }
}