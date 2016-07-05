namespace VSC.AST
{
    /// <summary>
    ///   Implementation of the `is' operator.
    /// </summary>
    public class IsExpression : ProbeExpression
    {

        public IsExpression(Expression expr, Expression probe_type, Location l)
            : base(expr, probe_type, l)
        {
        }


    }
}