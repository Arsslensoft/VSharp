using System;

namespace VSC.TypeSystem
{
    [Flags]
    public enum ParameterModifier
    {
        None,
        Ref,
        Out,
        Params,
        Self
    }
}