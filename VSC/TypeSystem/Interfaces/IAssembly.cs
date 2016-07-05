using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Represents an assembly.
	/// </summary>
	public interface IAssembly : ICompilationProvider
	{
		/// <summary>
		/// Gets the original unresolved assembly.
		/// </summary>
		IUnresolvedAssembly UnresolvedAssembly { get; }
		
	
		/// <summary>
		/// Gets the assembly name (short name).
		/// </summary>
		string AssemblyName { get; }
		
		
		/// <summary>
		/// Gets the list of all assembly attributes in the project.
		/// </summary>
		IList<IAttribute> AssemblyAttributes { get; }
		
		/// <summary>
		/// Gets the list of all module attributes in the project.
		/// </summary>
		IList<IAttribute> ModuleAttributes { get; }
		
		/// <summary>
		/// Gets whether the internals of this assembly are visible in the specified assembly.
		/// </summary>
		bool InternalsVisibleTo(IAssembly assembly);
		
		/// <summary>
		/// Gets the root namespace for this assembly.
		/// </summary>
		/// <remarks>
		/// This always is the namespace without a name - it's unrelated to the 'root namespace' project setting.
		/// </remarks>
		INamespace RootNamespace { get; }
		
		/// <summary>
		/// Gets the type definition for a top-level type.
		/// </summary>
		/// <remarks>This method uses ordinal name comparison, not the compilation's name comparer.</remarks>
		ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName);
		
		/// <summary>
		/// Gets all non-nested types in the assembly.
		/// </summary>
		IEnumerable<ITypeDefinition> TopLevelTypeDefinitions { get; }
	}
}
