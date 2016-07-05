namespace VSC.AST
{
    /// <summary>
    ///    A class used to assign values if the source expression is not void
    ///
    ///    Used by the interactive shell to allow it to call this code to set
    ///    the return value for an invocation.
    /// </summary>
    class OptionalAssign : SimpleAssign
    {
        public OptionalAssign(Expression s, Location loc)
            : base(null, s, loc)
        {
        }

     
    }
}