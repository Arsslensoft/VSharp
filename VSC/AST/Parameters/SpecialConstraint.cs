using System;

namespace VSC.AST
{
    [Flags]
    public enum SpecialConstraint
    {
        None = 0,
        Constructor = 1 << 2,
        Class = 1 << 3,
        Struct = 1 << 4
    }
}