namespace VSC.TypeSystem.Resolver
{
    /// <summary>
    /// Extension methods for the syntax tree.
    /// </summary>
    public static class ResolverExtensions
    {
        public static bool IsComparisonOperator(this OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.Equality:
                case OperatorType.Inequality:
                case OperatorType.GreaterThan:
                case OperatorType.LessThan:
                case OperatorType.GreaterThanOrEqual:
                case OperatorType.LessThanOrEqual:
                    return true;
                default:
                    return false;
            }
        }
    }
}