namespace VSC.TypeSystem
{
    /// <summary>
    /// Represents a field or constant.
    /// </summary>
    public interface IUnresolvedField : IUnresolvedMember
    {
        /// <summary>
        /// Gets whether this field is readonly.
        /// </summary>
        bool IsReadOnly { get; }
		
        /// <summary>
        /// Gets whether this field is volatile.
        /// </summary>
        bool IsVolatile { get; }
		
        /// <summary>
        /// Gets whether this field is a constant (V#-like const).
        /// </summary>
        bool IsConst { get; }

        /// <summary>
        /// Gets whether this field is a fixed size buffer (V#-like fixed).
        /// If this is true, then ConstantValue contains the size of the buffer.
        /// </summary>
        bool IsFixed { get; }


        IConstantValue ConstantValue { get; }
		
        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="context">
        /// Context for looking up the member. The context must specify the current assembly.
        /// A <see cref="SimpleTypeResolveContext"/> that specifies the current assembly is sufficient.
        /// </param>
        /// <returns>
        /// Returns the resolved member, or <c>null</c> if the member could not be found.
        /// </returns>
        new IField Resolve(ITypeResolveContext context);
    }
}