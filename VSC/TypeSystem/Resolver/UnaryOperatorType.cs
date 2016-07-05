namespace VSC.TypeSystem.Resolver
{
    public enum UnaryOperatorType
    {
        /// <summary>
        /// Any unary operator (used in pattern matching)
        /// </summary>
        Any,

        /// <summary>Logical not (!a)</summary>
        LogicalNot,
        /// <summary>Bitwise not (~a)</summary>
        OnesComplement,
        /// <summary>Unary minus (-a)</summary>
        UnaryNegation,
        /// <summary>Unary plus (+a)</summary>
        UnaryPlus,
        /// <summary>Pre increment (++a)</summary>
        PreIncrement,
        /// <summary>Pre decrement (--a)</summary>
        Decrement,
        /// <summary>Post increment (a++)</summary>
        PostIncrement,
        /// <summary>Post decrement (a--)</summary>
        PostDecrement,
        /// <summary>Dereferencing (*a)</summary>
        Dereference,
        /// <summary>Get address (&a)</summary>
        AddressOf,


        ///// <summary>Custom operator (@+-...a)</summary>
        //UnaryOperator,
    }
}