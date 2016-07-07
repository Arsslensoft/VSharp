using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a resolved namespace.
	/// </summary>
	public interface INamespace : ISymbol, ICompilationProvider
	{
		// No pointer back to unresolved namespace:
		// multiple unresolved namespaces (from different assemblies) get
		// merged into one INamespace.
		
		/// <summary>
		/// Gets the extern alias for this namespace.
		/// Returns an empty string for normal namespaces.
		/// </summary>
		string ExternAlias { get; }
		
		/// <summary>
        /// Gets the full name of this namespace. (e.g. "Std.Collections")
		/// </summary>
		string FullName { get; }
		
		/// <summary>
		/// Gets the short name of this namespace (e.g. "Collections").
		/// </summary>
		new string Name { get; }
		
		/// <summary>
		/// Gets the parent namespace.
		/// Returns null if this is the root namespace.
		/// </summary>
		INamespace ParentNamespace { get; }
		
		/// <summary>
		/// Gets the child namespaces in this namespace.
		/// </summary>
		IEnumerable<INamespace> ChildNamespaces { get; }
		
		/// <summary>
		/// Gets the types in this namespace.
		/// </summary>
		IEnumerable<ITypeDefinition> Types { get; }
		
		/// <summary>
		/// Gets the assemblies that contribute types to this namespace (or to child namespaces).
		/// </summary>
		IEnumerable<IAssembly> ContributingAssemblies { get; }
		
		/// <summary>
		/// Gets a direct child namespace by its short name.
		/// Returns null when the namespace cannot be found.
		/// </summary>
		/// <remarks>
		/// This method uses the compilation's current string comparer.
		/// </remarks>
		INamespace GetChildNamespace(string name);
		
		/// <summary>
		/// Gets the type with the specified short name and type parameter count.
		/// Returns null if the type cannot be found.
		/// </summary>
		/// <remarks>
		/// This method uses the compilation's current string comparer.
		/// </remarks>
		ITypeDefinition GetTypeDefinition(string name, int typeParameterCount);
	}
}
