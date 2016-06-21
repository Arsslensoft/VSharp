using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.TypeSystem.Interfaces;

namespace VSC.Context
{
    public interface ICodeContext : ITypeResolveContext
    {
        /// <summary>
        /// Gets all currently visible local variables and lambda parameters.
        /// Does not include method parameters.
        /// </summary>
        IEnumerable<IVariable> LocalVariables { get; }

        /// <summary>
        /// Gets whether the context is within a lambda expression or anonymous method.
        /// </summary>
        bool IsWithinLambdaExpression { get; }
    }
    public interface ITypeResolveContext : ICompilationProvider
    {
        /// <summary>
        /// Gets the current type definition.
        /// </summary>
        ITypeDefinition CurrentTypeDefinition { get; }

        /// <summary>
        /// Gets the current member.
        /// </summary>
        IMember CurrentMember { get; }

        ITypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition);
        ITypeResolveContext WithCurrentMember(IMember member);
    }
    public interface ICompilation
    {
        /// <summary>
        /// Gets the type resolve context that specifies this compilation and no current assembly or entity.
        /// </summary>
        ITypeResolveContext TypeResolveContext { get; }

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

        CacheManager CacheManager { get; }
    }

    public interface ICompilationProvider
    {
        /// <summary>
        /// Gets the parent compilation.
        /// This property never returns null.
        /// </summary>
        ICompilation Compilation { get; }
    }

    /// <summary>
    /// Represents a single file that was parsed.
    /// </summary>
    public interface IUnresolvedFile
    {
        /// <summary>
        /// Returns the full path of the file.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the time when the file was last written.
        /// </summary>
        DateTime? LastWriteTime { get; set; }

        /// <summary>
        /// Gets all top-level type definitions.
        /// </summary>
        IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions { get; }


        /// <summary>
        /// Gets all module attributes that are defined in this file.
        /// </summary>
        IList<IUnresolvedAttribute> ModuleAttributes { get; }

        /// <summary>
        /// Gets the top-level type defined at the specified location.
        /// Returns null if no type is defined at that location.
        /// </summary>
        IUnresolvedTypeDefinition GetTopLevelTypeDefinition(Location location);

        /// <summary>
        /// Gets the member defined at the specified location.
        /// Returns null if no member is defined at that location.
        /// </summary>
        IUnresolvedMember GetMember(Location location);

        /// <summary>
        /// Gets the parser errors.
        /// </summary>
        IList<ErrorMessage> Errors { get; }
    }
}
