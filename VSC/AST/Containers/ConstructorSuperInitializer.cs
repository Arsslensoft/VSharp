namespace VSC.AST
{
    public class ConstructorSuperInitializer : ConstructorInitializer
    {
        public ConstructorSuperInitializer(Arguments argument_list, Location l) :
            base(argument_list, l)
        {
        }
    }
}