namespace VSC.AST
{
    /// <summary>
    ///   Implementation of the `as' operator.
    /// </summary>
    public class AsExpression : ProbeExpression
    {

        public AsExpression(Expression expr, Expression probe_type, Location l)
            : base(expr, probe_type, l)
        {
        }


    }
}