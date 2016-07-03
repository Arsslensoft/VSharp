using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// References another project content in the same solution.
	/// Using the <see cref="ProjectReference"/> class requires that you 
	/// </summary>
	[Serializable]
	public class ProjectReference : IAssemblyReference
	{
		readonly string projectFileName;
		
		/// <summary>
		/// Creates a new reference to the specified project (must be part of the same solution).
		/// </summary>
		/// <param name="projectFileName">Full path to the file name. Must be identical to <see cref="IProjectContent.ProjectFileName"/> of the target project; do not use a relative path.</param>
		public ProjectReference(string projectFileName)
		{
			this.projectFileName = projectFileName;
		}
		
		public IAssembly Resolve(ITypeResolveContext context)
		{
			var solution = context.Compilation.SolutionSnapshot;
			var pc = solution.GetProjectContent(projectFileName);
			if (pc != null)
				return pc.Resolve(context);
			else
				return null;
		}
		
		public override string ToString()
		{
			return string.Format("[ProjectReference {0}]", projectFileName);
		}
	}
}
