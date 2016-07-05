namespace VSC.TypeSystem
{
    public interface ISymbolReference
    {
        ISymbol Resolve(ITypeResolveContext context);
    }
}