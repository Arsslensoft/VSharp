using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using VSC.Base;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Default implementation of ISolutionSnapshot.
	/// </summary>
	public class DefaultSolutionSnapshot : ISolutionSnapshot
	{
		readonly Dictionary<string, IProjectContent> projectDictionary = new Dictionary<string, IProjectContent>(VSC.Base.Platform.FileNameComparer);
		ConcurrentDictionary<IProjectContent, ICompilation> dictionary = new ConcurrentDictionary<IProjectContent, ICompilation>();
		
		/// <summary>
		/// Creates a new DefaultSolutionSnapshot with the specified projects.
		/// </summary>
		public DefaultSolutionSnapshot(IEnumerable<IProjectContent> projects)
		{
			foreach (var project in projects) {
				if (project.ProjectFileName != null)
					projectDictionary.Add(project.ProjectFileName, project);
			}
		}
		
		/// <summary>
		/// Creates a new DefaultSolutionSnapshot that does not support <see cref="ProjectReference"/>s.
		/// </summary>
		public DefaultSolutionSnapshot()
		{
		}
		
		public IProjectContent GetProjectContent(string projectFileName)
		{
			IProjectContent pc;
			lock (projectDictionary) {
				if (projectDictionary.TryGetValue(projectFileName, out pc))
					return pc;
				else
					return null;
			}
		}
		
		public ICompilation GetCompilation(IProjectContent project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			return dictionary.GetOrAdd(project, p => p.CreateCompilation(this));
		}
		
		public void AddCompilation(IProjectContent project, ICompilation compilation)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			if (!dictionary.TryAdd(project, compilation))
				throw new InvalidOperationException();
			if (project.ProjectFileName != null) {
				lock (projectDictionary) {
					projectDictionary.Add(project.ProjectFileName, project);
				}
			}
		}
	}
}
