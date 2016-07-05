namespace VSC.AST
{
    public class QueryBlock : ParametersBlock
    {
 
        public QueryBlock(Block parent, Location start)
            : base(parent, ParametersCompiled.EmptyReadOnlyParameters, start, Flags.CompilerGenerated)
        {
        }
        public void AddRangeVariable(RangeVariable variable)
        {
            variable.Block = this;
            TopBlock.AddLocalName(variable.Name, variable, true);
        }
    }
}