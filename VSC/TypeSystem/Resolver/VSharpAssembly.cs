using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem.Implementation;


namespace VSC.TypeSystem.Resolver
{
	public class VSharpAssembly : IAssembly
	{
		readonly ICompilation compilation;
		readonly ITypeResolveContext context;
		readonly VSharpProjectContent projectContent;
		IList<IAttribute> assemblyAttributes;
		IList<IAttribute> moduleAttributes;
		
		internal VSharpAssembly(ICompilation compilation, VSharpProjectContent projectContent)
		{
			this.compilation = compilation;
			this.projectContent = projectContent;
			this.context = new SimpleTypeResolveContext(this);
		}
		
		public bool IsMainAssembly {
			get { return compilation.MainAssembly == this; }
		}
		
		public IUnresolvedAssembly UnresolvedAssembly {
			get { return projectContent; }
		}
		
		public string AssemblyName {
			get { return projectContent.AssemblyName; }
		}
		
		public string FullAssemblyName {
			get { return projectContent.FullAssemblyName; }
		}
		
		public IList<IAttribute> AssemblyAttributes {
			get {
				return GetAttributes(ref assemblyAttributes, true);
			}
		}
		
		public IList<IAttribute> ModuleAttributes {
			get {
				return GetAttributes(ref moduleAttributes, false);
			}
		}
		
		IList<IAttribute> GetAttributes(ref IList<IAttribute> field, bool assemblyAttributes)
		{
			IList<IAttribute> result = LazyInit.VolatileRead(ref field);
			if (result != null) {
				return result;
			} else {
				result = new List<IAttribute>();
                foreach (var unresolvedFile in projectContent.Files.OfType<CompilationSourceFile>())
                {
					var attributes = assemblyAttributes ? unresolvedFile.AssemblyAttributes : unresolvedFile.ModuleAttributes;
					var context = new VSharpTypeResolveContext(this, unresolvedFile.RootUsingScope.ResolveScope(compilation));
					foreach (var unresolvedAttr in attributes) {
						result.Add(unresolvedAttr.CreateResolvedAttribute(context));
					}
				}
				return LazyInit.GetOrSet(ref field, result);
			}
		}
		
		NS rootNamespace;
		
		public INamespace RootNamespace {
			get {
				NS root = LazyInit.VolatileRead(ref this.rootNamespace);
				if (root != null) {
					return root;
				} else {
					root = new NS(this);
					Dictionary<string, NS> dict = new Dictionary<string, NS>(compilation.NameComparer);
					dict.Add(string.Empty, root);
					// Addition namespaces declared in V# files, even if they're empty:
					foreach (var usingScope in projectContent.Files.OfType<CompilationSourceFile>().SelectMany(f => f.UsingScopes)) {
						GetOrAddNamespace(dict, usingScope.NamespaceName);
					}
					foreach (var pair in GetTypes()) {
						NS ns = GetOrAddNamespace(dict, pair.Key.Namespace);
						if (ns.types != null)
							ns.types[pair.Key] = pair.Value;
					}
					return LazyInit.GetOrSet(ref this.rootNamespace, root);
				}
			}
		}
		
		static NS GetOrAddNamespace(Dictionary<string, NS> dict, string fullName)
		{
			NS ns;
			if (dict.TryGetValue(fullName, out ns))
				return ns;
			int pos = fullName.LastIndexOf('.');
			NS parent;
			string name;
			if (pos < 0) {
				parent = dict[string.Empty]; // root
				name = fullName;
			} else {
				parent = GetOrAddNamespace(dict, fullName.Substring(0, pos));
				name = fullName.Substring(pos + 1);
			}
			ns = new NS(parent, fullName, name);
			parent.childNamespaces.Add(ns);
			dict.Add(fullName, ns);
			return ns;
		}
		
		public ICompilation Compilation {
			get { return compilation; }
		}
		
		public bool InternalsVisibleTo(IAssembly assembly)
		{
			if (this == assembly)
				return true;
			foreach (string shortName in GetInternalsVisibleTo()) {
				if (assembly.AssemblyName == shortName)
					return true;
			}
			return false;
		}
		
		volatile string[] internalsVisibleTo;
		
		string[] GetInternalsVisibleTo()
		{
			var result = this.internalsVisibleTo;
			if (result != null) {
				return result;
			} else {
				using (var busyLock = BusyManager.Enter(this)) {
					Debug.Assert(busyLock.Success);
					if (!busyLock.Success) {
						return new string[0];
					}
					internalsVisibleTo = (
						from attr in this.AssemblyAttributes
						where attr.AttributeType.Name == "InternalsVisibleToAttribute"
                        && attr.AttributeType.Namespace == "Std.Runtime.CompilerServices"
						&& attr.PositionalArguments.Count == 1
						select GetShortName(attr.PositionalArguments.Single().ConstantValue as string)
					).ToArray();
				}
				return internalsVisibleTo;
			}
		}
		
		static string GetShortName(string fullAssemblyName)
		{
			if (fullAssemblyName == null)
				return null;
			int pos = fullAssemblyName.IndexOf(',');
			if (pos < 0)
				return fullAssemblyName;
			else
				return fullAssemblyName.Substring(0, pos);
		}
		
		Dictionary<TopLevelTypeName, ITypeDefinition> typeDict;
		
		Dictionary<TopLevelTypeName, ITypeDefinition> GetTypes()
		{
			var dict = LazyInit.VolatileRead(ref this.typeDict);
			if (dict != null) {
				return dict;
			} else {
				// Always use the ordinal comparer for the main dictionary so that partial classes
				// get merged correctly.
				// The compilation's comparer will be used for the per-namespace dictionaries.
				var comparer = TopLevelTypeNameComparer.Ordinal;
				dict = projectContent.TopLevelTypeDefinitions
					.GroupBy(t => new TopLevelTypeName(t.Namespace, t.Name, t.TypeParameters.Count), comparer)
					.ToDictionary(g => g.Key, g => CreateResolvedTypeDefinition(g.ToArray()), comparer);
				return LazyInit.GetOrSet(ref this.typeDict, dict);
			}
		}
		
		ITypeDefinition CreateResolvedTypeDefinition(IUnresolvedTypeDefinition[] parts)
		{
			return new ResolvedTypeDefinitionSpec(context, parts);
		}
		
		public ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName)
		{
			ITypeDefinition def;
			if (GetTypes().TryGetValue(topLevelTypeName, out def))
				return def;
			else
				return null;
		}
		
		public IEnumerable<ITypeDefinition> TopLevelTypeDefinitions {
			get {
				return GetTypes().Values;
			}
		}
		
		public override string ToString()
		{
			return "[VSharpAssembly " + this.AssemblyName + "]";
		}
		
		sealed class NS : INamespace
		{
			readonly VSharpAssembly assembly;
			readonly NS parentNamespace;
			readonly string fullName;
			readonly string name;
			internal readonly List<NS> childNamespaces = new List<NS>();
			internal readonly Dictionary<TopLevelTypeName, ITypeDefinition> types;
			
			public NS(VSharpAssembly assembly)
			{
				this.assembly = assembly;
				this.fullName = string.Empty;
				this.name = string.Empty;
				// Our main dictionary for the VSharpAssembly is using an ordinal comparer.
				// If the compilation's comparer isn't ordinal, we need to create a new dictionary with the compilation's comparer.
				if (assembly.compilation.NameComparer != StringComparer.Ordinal) {
					this.types = new Dictionary<TopLevelTypeName, ITypeDefinition>(new TopLevelTypeNameComparer(assembly.compilation.NameComparer));
				}
			}
			
			public NS(NS parentNamespace, string fullName, string name)
			{
				this.assembly = parentNamespace.assembly;
				this.parentNamespace = parentNamespace;
				this.fullName = fullName;
				this.name = name;
				if (parentNamespace.types != null)
					this.types = new Dictionary<TopLevelTypeName, ITypeDefinition>(parentNamespace.types.Comparer);
			}
			
			string INamespace.ExternAlias {
				get { return null; }
			}
			
			string INamespace.FullName {
				get { return fullName; }
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
				get { return childNamespaces; }
			}
			
			IEnumerable<ITypeDefinition> INamespace.Types {
				get {
					if (types != null)
						return types.Values;
					else
						return (
							from t in assembly.GetTypes()
							where t.Key.Namespace == fullName
							select t.Value
						);
				}
			}
			
			ICompilation ICompilationProvider.Compilation {
				get { return assembly.Compilation; }
			}
			
			IEnumerable<IAssembly> INamespace.ContributingAssemblies {
				get { return new [] { assembly }; }
			}
			
			INamespace INamespace.GetChildNamespace(string name)
			{
				var nameComparer = assembly.compilation.NameComparer;
				foreach (NS childNamespace in childNamespaces) {
					if (nameComparer.Equals(name, childNamespace.name))
						return childNamespace;
				}
				return null;
			}
			
			ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
			{
				var key = new TopLevelTypeName(fullName, name, typeParameterCount);
				if (types != null) {
					ITypeDefinition typeDef;
					if (types.TryGetValue(key, out typeDef))
						return typeDef;
					else
						return null;
				} else {
					return assembly.GetTypeDefinition(key);
				}
			}

			public ISymbolReference ToReference()
			{
				return new NamespaceReference(new AssemblyReferenceSpec(assembly.AssemblyName), fullName);
			}
		}
	}
}
