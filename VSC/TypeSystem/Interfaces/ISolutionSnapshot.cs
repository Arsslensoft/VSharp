using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a snapshot of the whole solution (multiple compilations).
	/// </summary>
	public interface ISolutionSnapshot
	{
		/// <summary>
		/// Gets the project content with the specified file name.
		/// Returns null if no such project exists in the solution.
		/// </summary>
		/// <remarks>
		/// This method is used by the <see cref="ProjectReference"/> class.
		/// </remarks>
		IProjectContent GetProjectContent(string projectFileName);
		
		/// <summary>
		/// Gets the compilation for the specified project.
		/// The project must be a part of the solution (passed to the solution snapshot's constructor).
		/// </summary>
		ICompilation GetCompilation(IProjectContent project);
	}
}
