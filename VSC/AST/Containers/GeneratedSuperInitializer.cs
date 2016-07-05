namespace VSC.AST
{
    sealed class GeneratedSuperInitializer : ConstructorSuperInitializer
    {
        public GeneratedSuperInitializer(Location loc, Arguments arguments)
            : base(arguments, loc)
        {
        }
    }
}