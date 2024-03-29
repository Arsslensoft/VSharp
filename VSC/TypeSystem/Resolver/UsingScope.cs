using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem.Implementation;


namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a scope that contains "using" statements.
	/// This is either the file itself, or a namespace declaration.
	/// </summary>
	[Serializable]
	public class UsingScope : FreezableSpec
	{
		readonly UsingScope parent;
		DomRegion region;
		string shortName = "";
		IList<TypeNameExpression> usings;
        IList<KeyValuePair<string, TypeNameExpression>> usingAliases;
		IList<string> externAliases;
		
		protected override void FreezeInternal()
		{
			usings = FreezableHelper.FreezeList(usings);
			usingAliases = FreezableHelper.FreezeList(usingAliases);
			externAliases = FreezableHelper.FreezeList(externAliases);
			
			// In current model (no child scopes), it makes sense to freeze the parent as well
			// to ensure the whole lookup chain is immutable.
			if (parent != null)
				parent.Freeze();
			
			base.FreezeInternal();
		}
		
		/// <summary>
		/// Creates a new root using scope.
		/// </summary>
		public UsingScope()
		{
		}
		
		/// <summary>
		/// Creates a new nested using scope.
		/// </summary>
		/// <param name="parent">The parent using scope.</param>
		/// <param name="shortName">The short namespace name.</param>
		public UsingScope(UsingScope parent, string shortName)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (shortName == null)
				throw new ArgumentNullException("shortName");
			this.parent = parent;
			this.shortName = shortName;
		}
		
		public UsingScope Parent {
			get { return parent; }
		}
		
		public DomRegion Region {
			get { return region; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				region = value;
			}
		}
		
		public string ShortNamespaceName {
			get {
				return shortName;
			}
		}
        public static string BuildQualifiedName(string name1, string name2)
        {
            if (string.IsNullOrEmpty(name1))
                return name2;
            if (string.IsNullOrEmpty(name2))
                return name1;
            return name1 + "." + name2;
        }
		public string NamespaceName {
			get {
				if (parent != null)
					return BuildQualifiedName(parent.NamespaceName, shortName);
				else
					return shortName;
			}
//			set {
//				if (value == null)
//					throw new ArgumentNullException("NamespaceName");
//				FreezableHelper.ThrowIfFrozen(this);
//				namespaceName = value;
//			}
		}

        public IList<TypeNameExpression> Usings
        {
			get {
				if (usings == null)
                    usings = new List<TypeNameExpression>();
				return usings;
			}
		}

        public IList<KeyValuePair<string, TypeNameExpression>> UsingAliases
        {
			get {
				if (usingAliases == null)
                    usingAliases = new List<KeyValuePair<string, TypeNameExpression>>();
				return usingAliases;
			}
		}
		
		public IList<string> ExternAliases {
			get {
				if (externAliases == null)
					externAliases = new List<string>();
				return externAliases;
			}
		}
		
//		public IList<UsingScope> ChildScopes {
//			get {
//				if (childScopes == null)
//					childScopes = new List<UsingScope>();
//				return childScopes;
//			}
//		}
		
		/// <summary>
		/// Gets whether this using scope has an alias (either using or extern)
		/// with the specified name.
		/// </summary>
		public bool HasAlias(string identifier)
		{
			if (usingAliases != null) {
				foreach (var pair in usingAliases) {
					if (pair.Key == identifier)
						return true;
				}
			}
			return externAliases != null && externAliases.Contains(identifier);
		}
		
		/// <summary>
		/// Resolves the namespace represented by this using scope.
		/// </summary>
		public ResolvedUsingScope ResolveScope(ICompilation compilation)
		{
			CacheManager cache = compilation.CacheManager;
			ResolvedUsingScope resolved = cache.GetShared(this) as ResolvedUsingScope;
			if (resolved == null) {
				var csContext = new VSharpTypeResolveContext(compilation.MainAssembly, parent != null ? parent.ResolveScope(compilation) : null);
				resolved = (ResolvedUsingScope)cache.GetOrAddShared(this, new ResolvedUsingScope(csContext, this));
			}
			return resolved;
		}
	}
}
