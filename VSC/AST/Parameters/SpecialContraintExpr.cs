namespace VSC.AST
{
    public class SpecialContraintExpr : FullNamedExpression
    {
        public SpecialContraintExpr(SpecialConstraint constraint, Location loc)
        {
            this.loc = loc;
            this.Constraint = constraint;
        }
        public SpecialConstraint Constraint { get; private set; }
    }
}