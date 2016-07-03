using System;

namespace VSC.TypeSystem
{
	public enum SymbolKind : byte
	{
		None,
		/// <seealso cref="ITypeDefinition"/>
		TypeDefinition,
		/// <seealso cref="IField"/>
		Field,
		/// <summary>
		/// The symbol is a property, but not an indexer.
		/// </summary>
		/// <seealso cref="IProperty"/>
		Property,
		/// <summary>
		/// The symbol is an indexer, not a regular property.
		/// </summary>
		/// <seealso cref="IProperty"/>
		Indexer,
		/// <seealso cref="IEvent"/>
		Event,
		/// <summary>
		/// The symbol is a method which is not an operator/constructor/destructor or accessor.
		/// </summary>
		/// <seealso cref="IMethod"/>
		Method,
		/// <summary>
		/// The symbol is a user-defined operator.
		/// </summary>
		/// <seealso cref="IMethod"/>
		Operator,
		/// <seealso cref="IMethod"/>
		Constructor,
		/// <seealso cref="IMethod"/>
		Destructor,
		/// <summary>
		/// The accessor method for a property getter/setter or event add/remove.
		/// </summary>
		/// <seealso cref="IMethod"/>
		Accessor,
		/// <seealso cref="INamespace"/>
		Namespace,
		/// <summary>
		/// The symbol is a variable, but not a parameter.
		/// </summary>
		/// <seealso cref="IVariable"/>
		Variable,
		/// <seealso cref="IParameter"/>
		Parameter,
		/// <seealso cref="ITypeParameter"/>
		TypeParameter,
	}
	
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
	
	public interface ISymbolReference
	{
		ISymbol Resolve(ITypeResolveContext context);
	}
}
