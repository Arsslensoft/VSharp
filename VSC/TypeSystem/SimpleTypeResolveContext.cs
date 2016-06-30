using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Default ITypeResolveContext implementation.
	/// </summary>
	public class SimpleTypeResolveContext : ITypeResolveContext
	{
		readonly ICompilation compilation;
		readonly IAssembly currentAssembly;
		readonly ITypeDefinition currentTypeDefinition;
		readonly IMember currentMember;
		
		public SimpleTypeResolveContext(ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			this.compilation = compilation;
		}
		
		public SimpleTypeResolveContext(IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			this.compilation = assembly.Compilation;
			this.currentAssembly = assembly;
		}
		
		public SimpleTypeResolveContext(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			this.compilation = entity.Compilation;
			this.currentAssembly = entity.ParentAssembly;
			this.currentTypeDefinition = (entity as ITypeDefinition) ?? entity.DeclaringTypeDefinition;
			this.currentMember = entity as IMember;
		}
		
		private SimpleTypeResolveContext(ICompilation compilation, IAssembly currentAssembly, ITypeDefinition currentTypeDefinition, IMember currentMember)
		{
			this.compilation = compilation;
			this.currentAssembly = currentAssembly;
			this.currentTypeDefinition = currentTypeDefinition;
			this.currentMember = currentMember;
		}
		
		public ICompilation Compilation {
			get { return compilation; }
		}
		
		public IAssembly CurrentAssembly {
			get { return currentAssembly; }
		}
		
		public ITypeDefinition CurrentTypeDefinition {
			get { return currentTypeDefinition; }
		}
		
		public IMember CurrentMember {
			get { return currentMember; }
		}
		
		public ITypeResolveContext WithCurrentTypeDefinition(ITypeDefinition typeDefinition)
		{
			return new SimpleTypeResolveContext(compilation, currentAssembly, typeDefinition, currentMember);
		}
		
		public ITypeResolveContext WithCurrentMember(IMember member)
		{
			return new SimpleTypeResolveContext(compilation, currentAssembly, currentTypeDefinition, member);
		}
	}
}
