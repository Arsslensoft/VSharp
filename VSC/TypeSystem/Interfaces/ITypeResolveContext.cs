namespace VSC.TypeSystem
{
    public interface ITypeResolveContext : ICompilationProvider
    {
        /// <summary>
        /// Gets the current assembly.
        /// This property may return null if this context does not specify any assembly.
        /// </summary>
        IAssembly CurrentAssembly { get; }
		
        /// <summary>
        /// Gets the current type definition.
        /// </summary>
        ITypeDefinition CurrentTypeDefinition { get ;}
		
        /// <summary>
        /// Gets the current member.
        /// </summary>
        IMember CurrentMember { get; }
		
        ITypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition);
        ITypeResolveContext WithCurrentMember(IMember member);
    }
}