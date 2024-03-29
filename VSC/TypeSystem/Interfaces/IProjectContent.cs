using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents an assembly consisting of source code (parsed files).
	/// </summary>
	public interface IProjectContent : IUnresolvedAssembly
	{
		/// <summary>
		/// Gets the path to the project file (e.g. .csproj).
		/// </summary>
		string ProjectFileName { get; }
		
		/// <summary>
		/// Gets a parsed file by its file name.
		/// </summary>
		IUnresolvedFile GetFile(string fileName);
		
		/// <summary>
		/// Gets the list of all files in the project content.
		/// </summary>
		IEnumerable<IUnresolvedFile> Files { get; }
		
		/// <summary>
		/// Gets the referenced assemblies.
		/// </summary>
		IEnumerable<IAssemblyReference> AssemblyReferences { get; }
		
		/// <summary>
		/// Gets the compiler settings object.
		/// The concrete type of the settings object depends on the programming language used to implement this project.
		/// </summary>
		object CompilerSettings { get; }
		
		/// <summary>
		/// Creates a new <see cref="ICompilation"/> that allows resolving within this project.
		/// </summary>
		/// <remarks>
		/// This method does not support <see cref="ProjectReference"/>s. When dealing with a solution
		/// containing multiple projects, consider using <see cref="ISolutionSnapshot.GetCompilation"/> instead.
		/// </remarks>
		ICompilation CreateCompilation();
		
		/// <summary>
		/// Creates a new <see cref="ICompilation"/> that allows resolving within this project.
		/// </summary>
		/// <param name="solutionSnapshot">The parent solution snapshot to use for the compilation.</param>
		/// <remarks>
		/// This method is intended to be called by ISolutionSnapshot implementations. Other code should
		/// call <see cref="ISolutionSnapshot.GetCompilation"/> instead.
		/// This method always creates a new compilation, even if the solution snapshot already contains
		/// one for this project.
		/// </remarks>
		ICompilation CreateCompilation(ISolutionSnapshot solutionSnapshot);
		
		/// <summary>
		/// Changes the assembly name of this project content.
		/// </summary>
		IProjectContent SetAssemblyName(string newAssemblyName);

		/// <summary>
		/// Changes the project file name of this project content.
		/// </summary>
		IProjectContent SetProjectFileName(string newProjectFileName);
		
		/// <summary>
		/// Changes the path to the assembly location (the output path where the project compiles to).
		/// </summary>
		IProjectContent SetLocation(string newLocation);

		/// <summary>
		/// Addition assembly references to this project content.
		/// </summary>
		IProjectContent AddAssemblyReferences(IEnumerable<IAssemblyReference> references);
		
		/// <summary>
		/// Addition assembly references to this project content.
		/// </summary>
		IProjectContent AddAssemblyReferences(params IAssemblyReference[] references);
		
		/// <summary>
		/// Removes assembly references from this project content.
		/// </summary>
		IProjectContent RemoveAssemblyReferences(IEnumerable<IAssemblyReference> references);
		
		/// <summary>
		/// Removes assembly references from this project content.
		/// </summary>
		IProjectContent RemoveAssemblyReferences(params IAssemblyReference[] references);
		
		/// <summary>
		/// Adds the specified files to the project content.
		/// If a file with the same name already exists, updated the existing file.
		/// </summary>
		/// <remarks>
		/// You can create an unresolved file by calling <c>ToTypeSystem()</c> on a syntax tree.
		/// </remarks>
		IProjectContent AddOrUpdateFiles(IEnumerable<IUnresolvedFile> newFiles);
		
		/// <summary>
		/// Adds the specified files to the project content.
		/// If a file with the same name already exists, this method updates the existing file.
		/// </summary>
		/// <remarks>
		/// You can create an unresolved file by calling <c>ToTypeSystem()</c> on a syntax tree.
		/// </remarks>
		IProjectContent AddOrUpdateFiles(params IUnresolvedFile[] newFiles);
		
		/// <summary>
		/// Removes the files with the specified names.
		/// </summary>
		IProjectContent RemoveFiles(IEnumerable<string> fileNames);
		
		/// <summary>
		/// Removes the files with the specified names.
		/// </summary>
		IProjectContent RemoveFiles(params string[] fileNames);
		
		/// <summary>
		/// Removes types and attributes from oldFile from the project, and adds those from newFile.
		/// </summary>
		[Obsolete("Use RemoveFiles()/AddOrUpdateFiles() instead")]
		IProjectContent UpdateProjectContent(IUnresolvedFile oldFile, IUnresolvedFile newFile);
		
		/// <summary>
		/// Removes types and attributes from oldFiles from the project, and adds those from newFiles.
		/// </summary>
		[Obsolete("Use RemoveFiles()/AddOrUpdateFiles() instead")]
		IProjectContent UpdateProjectContent(IEnumerable<IUnresolvedFile> oldFiles, IEnumerable<IUnresolvedFile> newFiles);
		
		/// <summary>
		/// Sets the compiler settings object.
		/// The concrete type of the settings object depends on the programming language used to implement this project.
		/// Using the incorrect type of settings object results in an <see cref="ArgumentException"/>.
		/// </summary>
		IProjectContent SetCompilerSettings(object compilerSettings);
	}
}
