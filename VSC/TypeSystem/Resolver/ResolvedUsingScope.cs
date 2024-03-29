using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VSC.AST;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem.Implementation;
using Expression = VSC.AST.Expression;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Resolved version of using scope.
	/// </summary>
	public class ResolvedUsingScope
	{
		readonly VSharpTypeResolveContext parentContext;
		readonly UsingScope usingScope;

        internal readonly ConcurrentDictionary<string, Expression> ResolveCache = new ConcurrentDictionary<string, Expression>();
		internal List<List<IMethod>> AllExtensionMethods;
		
		public ResolvedUsingScope(VSharpTypeResolveContext context, UsingScope usingScope)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (usingScope == null)
				throw new ArgumentNullException("usingScope");
			this.parentContext = context;
			this.usingScope = usingScope;
			if (usingScope.Parent != null) {
				if (context.CurrentUsingScope == null)
					throw new InvalidOperationException();
			} else {
				if (context.CurrentUsingScope != null)
					throw new InvalidOperationException();
			}
		}
		
		public UsingScope UnresolvedUsingScope {
			get { return usingScope; }
		}
		
		INamespace @namespace;
		
		public INamespace Namespace {
			get {
				INamespace result = LazyInit.VolatileRead(ref this.@namespace);
				if (result != null) {
					return result;
				} else {
					if (parentContext.CurrentUsingScope != null) {
						result = parentContext.CurrentUsingScope.Namespace.GetChildNamespace(usingScope.ShortNamespaceName);
						if (result == null)
							result = new DummyNamespace(parentContext.CurrentUsingScope.Namespace, usingScope.ShortNamespaceName);
					} else {
						result = parentContext.Compilation.RootNamespace;
					}
					Debug.Assert(result != null);
					return LazyInit.GetOrSet(ref this.@namespace, result);
				}
			}
		}
		
		public ResolvedUsingScope Parent {
			get { return parentContext.CurrentUsingScope; }
		}
		
		IList<INamespace> usings;
		
		public IList<INamespace> Usings {
			get {
				var result = LazyInit.VolatileRead(ref this.usings);
				if (result != null) {
					return result;
				} else {
					result = new List<INamespace>();
                    ResolveContext resolver = new ResolveContext(parentContext.WithUsingScope(this), CompilerContext.report);
					foreach (var u in usingScope.Usings) {
						INamespace ns = u.ResolveNamespace(resolver);
						if (ns != null && !result.Contains(ns))
							result.Add(ns);
					}
					return LazyInit.GetOrSet(ref this.usings, new ReadOnlyCollection<INamespace>(result));
				}
			}
		}

        IList<KeyValuePair<string, Expression>> usingAliases;
		
		public IList<KeyValuePair<string, Expression>> UsingAliases {
			get {
				var result = LazyInit.VolatileRead(ref this.usingAliases);
				if (result != null) {
					return result;
				} else {
                    ResolveContext resolver = new ResolveContext(parentContext.WithUsingScope(this), CompilerContext.report);
                    result = new KeyValuePair<string, Expression>[usingScope.UsingAliases.Count];
					for (int i = 0; i < result.Count; i++) {
						var rr = usingScope.UsingAliases[i].Value.Resolve(resolver);
                        //if (rr is TypeResolveResult) {
                        //    rr = new AliasTypeResolveResult (usingScope.UsingAliases[i].Key, (TypeResolveResult)rr);
                        //} else if (rr is NamespaceResolveResult) {
                        //    rr = new AliasNamespace(usingScope.UsingAliases[i].Key, (AliasNamespace)rr);
                        //}
                        result[i] = new KeyValuePair<string, Expression>(
							usingScope.UsingAliases[i].Key,
							rr
						);
					}
					return LazyInit.GetOrSet(ref this.usingAliases, result);
				}
			}
		}
		
		public IList<string> ExternAliases {
			get { return usingScope.ExternAliases; }
		}
		
		/// <summary>
		/// Gets whether this using scope has an alias (either using or extern)
		/// with the specified name.
		/// </summary>
		public bool HasAlias(string identifier)
		{
			return usingScope.HasAlias(identifier);
		}
		
		sealed class DummyNamespace : INamespace
		{
			readonly INamespace parentNamespace;
			readonly string name;
			
			public DummyNamespace(INamespace parentNamespace, string name)
			{
				this.parentNamespace = parentNamespace;
				this.name = name;
			}
			
			public string ExternAlias { get; set; }
			
			string INamespace.FullName {
				get { return UsingScope.BuildQualifiedName(parentNamespace.FullName, name); }
			}
			
			public string Name {
				get { return name; }
			}
			
			SymbolKind ISymbol.SymbolKind {
				get { return SymbolKind.Namespace; }
			}
			
			INamespace INamespace.ParentNamespace {
				get { return parentNamespace; }
			}
			
			IEnumerable<INamespace> INamespace.ChildNamespaces {
				get { return EmptyList<INamespace>.Instance; }
			}
			
			IEnumerable<ITypeDefinition> INamespace.Types {
				get { return EmptyList<ITypeDefinition>.Instance; }
			}
			
			IEnumerable<IAssembly> INamespace.ContributingAssemblies {
				get { return EmptyList<IAssembly>.Instance; }
			}
			
			ICompilation ICompilationProvider.Compilation {
				get { return parentNamespace.Compilation; }
			}
			
			INamespace INamespace.GetChildNamespace(string name)
			{
				return null;
			}
			
			ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
			{
				return null;
			}

			public ISymbolReference ToReference()
			{
				return new MergedNamespaceReference(ExternAlias, ((INamespace)this).FullName);
			}
		}
	}
}
