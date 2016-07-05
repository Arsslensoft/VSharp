namespace VSC.AST
{
    public class DeclarationExpression : Expression
    {
        public DeclarationExpression(FullNamedExpression variableType, LocalVariable variable)
        {
            VariableType = variableType;
            Variable = variable;
            this.loc = variable.Location;
        }

        public LocalVariable Variable { get; set; }
        public Expression Initializer { get; set; }
        public FullNamedExpression VariableType { get; set; }


    }
}