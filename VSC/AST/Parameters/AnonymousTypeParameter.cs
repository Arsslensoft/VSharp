namespace VSC.AST
{
    public class AnonymousTypeParameter : ShimExpression
    {
        public readonly string Name;

        public AnonymousTypeParameter(Expression initializer, string name, Location loc)
            : base(initializer)
        {
            this.Name = name;
            this.loc = loc;
        }

        public AnonymousTypeParameter(Parameter parameter)
            : base(new SimpleName(parameter.Name, parameter.Location))
        {
            this.Name = parameter.Name;
            this.loc = parameter.Location;
        }


    }
}