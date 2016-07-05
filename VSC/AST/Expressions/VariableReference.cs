namespace VSC.AST
{
    public abstract class VariableReference : Expression
    {
        public abstract string Name { get; }
    }
}