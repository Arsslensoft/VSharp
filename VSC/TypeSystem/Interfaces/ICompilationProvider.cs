namespace VSC.TypeSystem
{
    public interface ICompilationProvider
    {
        /// <summary>
        /// Gets the parent compilation.
        /// This property never returns null.
        /// </summary>
        ICompilation Compilation { get; }
    }
}