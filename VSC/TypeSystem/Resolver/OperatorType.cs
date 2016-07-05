namespace VSC.TypeSystem.Resolver
{
    public enum OperatorType
    {



        // Unary operators
        LogicalNot ,
        OnesComplement,
        Increment,
        Decrement,
        True ,
        False,

        // Unary and Binary operators
        Addition ,
        Subtraction,

        UnaryPlus,
        UnaryNegation ,

        // Binary operators
        Multiply,
        Division,
        Modulus,
        BitwiseAnd,
        BitwiseOr,
        ExclusiveOr ,
        LeftShift,
        RightShift,
        LeftRotate,
        RightRotate,
        Equality ,
        Inequality,
        GreaterThan,
        LessThan ,
        GreaterThanOrEqual,
        LessThanOrEqual,

        // Implicit and Explicit
        Implicit,
        Explicit,
        // Pattern matching
        Is,
        //BinaryOperatorConstant,
        //UnaryOperatorConstant,
        // Just because of enum
        TOP
    }
}