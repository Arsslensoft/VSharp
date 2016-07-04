using System;
using System.Collections.Generic;

namespace VSC.TypeSystem
{
	[Obsolete("IParsedFile was renamed to IUnresolvedFile", true)]
	public interface IParsedFile {}
	
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
		/// Gets all assembly attributes that are defined in this file.
		/// </summary>
		IList<IUnresolvedAttribute> AssemblyAttributes { get; }
		
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
		/// Gets the type (potentially a nested type) defined at the specified location.
		/// Returns null if no type is defined at that location.
		/// </summary>
		IUnresolvedTypeDefinition GetInnermostTypeDefinition(Location location);
		
		/// <summary>
		/// Gets the member defined at the specified location.
		/// Returns null if no member is defined at that location.
		/// </summary>
		IUnresolvedMember GetMember(Location location);
		

	}
}
