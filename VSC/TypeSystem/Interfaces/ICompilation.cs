using System;
using System.Collections.Generic;
using VSC.Base;

namespace VSC.TypeSystem
{
	public interface ICompilation
	{
		/// <summary>
		/// Gets the current assembly.
		/// </summary>
		IAssembly MainAssembly { get; }
		
		/// <summary>
		/// Gets the type resolve context that specifies this compilation and no current assembly or entity.
		/// </summary>
		ITypeResolveContext TypeResolveContext { get; }
		
		/// <summary>
		/// Gets the list of all assemblies in the compilation.
		/// </summary>
		/// <remarks>
		/// This main assembly is the first entry in the list.
		/// </remarks>
		IList<IAssembly> Assemblies { get; }
		
		/// <summary>
		/// Gets the referenced assemblies.
		/// This list does not include the main assembly.
		/// </summary>
		IList<IAssembly> ReferencedAssemblies { get; }
		
		/// <summary>
		/// Gets the root namespace of this compilation.
		/// This is a merged version of the root namespaces of all assemblies.
		/// </summary>
		/// <remarks>
		/// This always is the namespace without a name - it's unrelated to the 'root namespace' project setting.
		/// </remarks>
		INamespace RootNamespace { get; }
		
		/// <summary>
		/// Gets the root namespace for a given extern alias.
		/// </summary>
		/// <remarks>
		/// If <paramref name="alias"/> is <c>null</c> or an empty string, this method
		/// returns the global root namespace.
		/// If no alias with the specified name exists, this method returns null.
		/// </remarks>
		INamespace GetNamespaceForExternAlias(string alias);
		
		IType FindType(KnownTypeCode typeCode);
		
		/// <summary>
		/// Gets the name comparer for the language being compiled.
		/// This is the string comparer used for the INamespace.GetTypeDefinition method.
		/// </summary>
		StringComparer NameComparer { get; }
		
		ISolutionSnapshot SolutionSnapshot { get; }
		
		CacheManager CacheManager { get; }
	}
}
