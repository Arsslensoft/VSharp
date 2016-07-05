namespace VSC.AST
{
    public class VarTypeExpression : SimpleName
    {
        public VarTypeExpression(Location loc)
            : base("var", loc)
        {
        }


    }
}