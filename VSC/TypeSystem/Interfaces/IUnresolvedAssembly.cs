using System.Collections.Generic;

namespace VSC.TypeSystem
{
    /// <summary>
    /// Represents an unresolved assembly.
    /// </summary>
    public interface IUnresolvedAssembly : IAssemblyReference
    {
        /// <summary>
        /// Gets the assembly name (short name).
        /// </summary>
        string AssemblyName { get; }
		
        /// <summary>
        /// Gets the full assembly name (including public key token etc.)
        /// </summary>
        string FullAssemblyName { get; }
		
        /// <summary>
        /// Gets the path to the assembly location. 
        /// For projects it is the same as the output path.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Gets the list of all assembly attributes in the project.
        /// </summary>
        IEnumerable<IUnresolvedAttribute> AssemblyAttributes { get; }
		
        /// <summary>
        /// Gets the list of all module attributes in the project.
        /// </summary>
        IEnumerable<IUnresolvedAttribute> ModuleAttributes { get; }
		
        /// <summary>
        /// Gets all non-nested types in the assembly.
        /// </summary>
        IEnumerable<IUnresolvedTypeDefinition> TopLevelTypeDefinitions { get; }
    }
}