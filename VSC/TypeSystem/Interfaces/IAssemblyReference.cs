namespace VSC.TypeSystem
{
    public interface IAssemblyReference
    {
        /// <summary>
        /// Resolves this assembly.
        /// </summary>
        IAssembly Resolve(ITypeResolveContext context);
    }
}