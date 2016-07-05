namespace VSC.TypeSystem.Resolver
{
    public enum BinaryOperatorType
    {
        /// <summary>
        /// Any binary operator (used in pattern matching)
        /// </summary>
        Any,

        // We avoid 'logical or' on purpose, because it's not clear if that refers to the bitwise
        // or to the short-circuiting (conditional) operator:
        // MCS and old NRefactory used bitwise='|', logical='||'
        // but the V# spec uses logical='|', conditional='||'
        /// <summary>left &amp; right</summary>
        BitwiseAnd,
        /// <summary>left | right</summary>
        BitwiseOr,
        /// <summary>left &amp;&amp; right</summary>
        LogicalAnd,
        /// <summary>left || right</summary>
        LogicalOr,
        /// <summary>left ^ right</summary>
        ExclusiveOr,

        /// <summary>left &gt; right</summary>
        GreaterThan,
        /// <summary>left &gt;= right</summary>
        GreaterThanOrEqual,
        /// <summary>left == right</summary>
        Equality,
        /// <summary>left != right</summary>
        Inequality,
        /// <summary>left &lt; right</summary>
        LessThan,
        /// <summary>left &lt;= right</summary>
        LessThanOrEqual,

        /// <summary>left + right</summary>
        Addition,
        /// <summary>left - right</summary>
        Subtraction,
        /// <summary>left * right</summary>
        Multiply,
        /// <summary>left / right</summary>
        Division,
        /// <summary>left % right</summary>
        Modulus,

        /// <summary>left &lt;&lt; right</summary>
        LeftShift,
        /// <summary>left &gt;&gt; right</summary>
        RightShift,


        /// <summary>left &lt;~ right</summary>
        RotateLeft,
        /// <summary>left ~&gt; right</summary>
        RotateRight,

        /// <summary>left ?? right</summary>
        NullCoalescing,
        /// <summary>left is right</summary>
        Is
    }
}