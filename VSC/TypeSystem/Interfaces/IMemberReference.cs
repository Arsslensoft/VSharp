namespace VSC.TypeSystem
{
    public interface IMemberReference : ISymbolReference
    {
        /// <summary>
        /// Gets the declaring type reference for the member.
        /// </summary>
        ITypeReference DeclaringTypeReference { get; }
		
        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="context">
        /// Context to use for resolving this member reference.
        /// Which kind of context is required depends on the which kind of member reference this is;
        /// please consult the documentation of the method that was used to create this member reference,
        /// or that of the class implementing this method.
        /// </param>
        /// <returns>
        /// Returns the resolved member, or <c>null</c> if the member could not be found.
        /// </returns>
        new IMember Resolve(ITypeResolveContext context);
    }
}