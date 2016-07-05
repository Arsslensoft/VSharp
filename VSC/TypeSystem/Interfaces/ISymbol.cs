using System;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Interface for type system symbols.
	/// </summary>
	public interface ISymbol
	{
		/// <summary>
		/// This property returns an enum specifying which kind of symbol this is
		/// (which derived interfaces of ISymbol are implemented)
		/// </summary>
		SymbolKind SymbolKind { get; }
		
		/// <summary>
		/// Gets the short name of the symbol.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Creates a symbol reference that can be used to rediscover this symbol in another compilation.
		/// </summary>
		ISymbolReference ToReference();
	}
}
