namespace VSC.TypeSystem
{
    /// <summary>
    /// Type parameter of a generic class/method.
    /// </summary>
    public interface IUnresolvedTypeParameter : INamedElement
    {
        /// <summary>
        /// Get the type of this type parameter's owner.
        /// </summary>
        /// <returns>SymbolKind.TypeDefinition or SymbolKind.Method</returns>
        SymbolKind OwnerType { get; }
		
        /// <summary>
        /// Gets the index of the type parameter in the type parameter list of the owning method/class.
        /// </summary>
        int Index { get; }
		
	
        /// <summary>
        /// Gets the region where the type parameter is defined.
        /// </summary>
        DomRegion Region { get; }
		
        ITypeParameter CreateResolvedTypeParameter(ITypeResolveContext context);
    }
}