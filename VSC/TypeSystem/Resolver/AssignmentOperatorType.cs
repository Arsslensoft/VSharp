namespace VSC.TypeSystem.Resolver
{
    public enum AssignmentOperatorType
    {
        /// <summary>left = right</summary>
        Assign,

        /// <summary>left += right</summary>
        Add,
        /// <summary>left -= right</summary>
        Subtract,
        /// <summary>left *= right</summary>
        Multiply,
        /// <summary>left /= right</summary>
        Divide,
        /// <summary>left %= right</summary>
        Modulus,

        /// <summary>left <<= right</summary>
        ShiftLeft,
        /// <summary>left >>= right</summary>
        ShiftRight,

        /// <summary>left <~= right</summary>
        RotateLeft,
        /// <summary>left ~>= right</summary>
        RotateRight,


        /// <summary>left &= right</summary>
        BitwiseAnd,
        /// <summary>left |= right</summary>
        BitwiseOr,
        /// <summary>left ^= right</summary>
        ExclusiveOr,

        /// <summary>Any operator (for pattern matching)</summary>
        Any
    }
}