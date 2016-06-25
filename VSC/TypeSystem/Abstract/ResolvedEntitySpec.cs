using System;
using System.Collections.Generic;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Implementation of <see cref="IEntity"/> that resolves an unresolved entity.
	/// </summary>
	public abstract class ResolvedEntitySpec : IEntity
	{
		protected readonly IUnresolvedEntity unresolved;
		protected readonly ITypeResolveContext parentContext;
		
		protected ResolvedEntitySpec(IUnresolvedEntity unresolved, ITypeResolveContext parentContext)
		{
			if (unresolved == null)
				throw new ArgumentNullException("unresolved");
			if (parentContext == null)
				throw new ArgumentNullException("parentContext");
			this.unresolved = unresolved;
			this.parentContext = parentContext;
			this.Attributes = unresolved.Attributes.CreateResolvedAttributes(parentContext);
		}
		
		public SymbolKind SymbolKind {
			get { return unresolved.SymbolKind; }
		}
		

		public DomRegion Region {
			get { return unresolved.Region; }
		}
		
		public DomRegion BodyRegion {
			get { return unresolved.BodyRegion; }
		}
		
		public ITypeDefinition DeclaringTypeDefinition {
			get { return parentContext.CurrentTypeDefinition; }
		}
		
		public virtual IType DeclaringType {
			get { return parentContext.CurrentTypeDefinition; }
		}
		
		public IAssembly ParentAssembly {
			get { return parentContext.CurrentAssembly; }
		}
		
		public IList<IAttribute> Attributes { get; protected set; }
		
	
		public abstract ISymbolReference ToReference();
		
		public bool IsStatic { get { return unresolved.IsStatic; } }
		public bool IsAbstract { get { return unresolved.IsAbstract; } }
		public bool IsSealed { get { return unresolved.IsSealed; } }
		public bool IsShadowing { get { return unresolved.IsShadowing; } }
		public bool IsSynthetic { get { return unresolved.IsSynthetic; } }
		
		public ICompilation Compilation {
			get { return parentContext.Compilation; }
		}
		
		public string FullName { get { return unresolved.FullName; } }
		public string Name { get { return unresolved.Name; } }
		public string ReflectionName { get { return unresolved.ReflectionName; } }
		public string Namespace { get { return unresolved.Namespace; } }
		
		public virtual Accessibility Accessibility { get { return unresolved.Accessibility; } }
		public bool IsPrivate { get { return Accessibility == Accessibility.Private; } }
		public bool IsPublic { get { return Accessibility == Accessibility.Public; } }
		public bool IsProtected { get { return Accessibility == Accessibility.Protected; } }
		public bool IsInternal { get { return Accessibility == Accessibility.Internal; } }
		public bool IsProtectedOrInternal { get { return Accessibility == Accessibility.ProtectedOrInternal; } }
		public bool IsProtectedAndInternal { get { return Accessibility == Accessibility.ProtectedAndInternal; } }
		
		public override string ToString()
		{
			return "[" + this.SymbolKind.ToString() + " " + this.ReflectionName + "]";
		}
	}
}
