namespace VSC.TypeSystem
{
    /// <summary>
    /// Represents an unresolved attribute.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public interface IUnresolvedAttribute
    {
        /// <summary>
        /// Gets the code region of this attribute.
        /// </summary>
        DomRegion Region { get; }
		
        /// <summary>
        /// Resolves the attribute.
        /// </summary>
        IAttribute CreateResolvedAttribute(ITypeResolveContext context);
    }
}