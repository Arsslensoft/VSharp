namespace VSC.AST
{
    public sealed class ConstructorSelfInitializer : ConstructorInitializer
    {
        public ConstructorSelfInitializer(Arguments argument_list, Location l) :
            base(argument_list, l)
        {
        }
    }
}