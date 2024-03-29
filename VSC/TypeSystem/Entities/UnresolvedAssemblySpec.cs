using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation for <see cref="IUnresolvedAssembly"/>.
	/// </summary>
	[Serializable]
	public class UnresolvedAssemblySpec : FreezableSpec, IUnresolvedAssembly
	{
		string assemblyName;
		string fullAssemblyName;
		IList<IUnresolvedAttribute> assemblyAttributes;
		IList<IUnresolvedAttribute> moduleAttributes;
		Dictionary<TopLevelTypeName, IUnresolvedTypeDefinition> typeDefinitions = new Dictionary<TopLevelTypeName, IUnresolvedTypeDefinition>(TopLevelTypeNameComparer.Ordinal);
		Dictionary<TopLevelTypeName, ITypeReference> typeForwarders = new Dictionary<TopLevelTypeName, ITypeReference>(TopLevelTypeNameComparer.Ordinal);
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
			assemblyAttributes = FreezableHelper.FreezeListAndElements(assemblyAttributes);
			moduleAttributes = FreezableHelper.FreezeListAndElements(moduleAttributes);
			foreach (var type in typeDefinitions.Values) {
				FreezableHelper.Freeze(type);
			}
		}
		
		/// <summary>
		/// Creates a new unresolved assembly.
		/// </summary>
		/// <param name="assemblyName">Full assembly name</param>
		public UnresolvedAssemblySpec(string assemblyName)
		{
			if (assemblyName == null)
				throw new ArgumentNullException("assemblyName");
			this.fullAssemblyName = assemblyName;
			int pos = assemblyName != null ? assemblyName.IndexOf(',') : -1;
			this.assemblyName = pos < 0 ? assemblyName : assemblyName.Substring(0, pos);
			this.assemblyAttributes = new List<IUnresolvedAttribute>();
			this.moduleAttributes = new List<IUnresolvedAttribute>();
		}
		
		/// <summary>
		/// Gets/Sets the short assembly name.
		/// </summary>
		/// <remarks>
		/// This class handles the short and the full name independently;
		/// if you change the short name, you should also change the full name.
		/// </remarks>
		public string AssemblyName {
			get { return assemblyName; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				FreezableHelper.ThrowIfFrozen(this);
				assemblyName = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets the full assembly name.
		/// </summary>
		/// <remarks>
		/// This class handles the short and the full name independently;
		/// if you change the full name, you should also change the short name.
		/// </remarks>
		public string FullAssemblyName {
			get { return fullAssemblyName; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				FreezableHelper.ThrowIfFrozen(this);
				fullAssemblyName = value;
			}
		}
		
		string location;
		public string Location {
			get {
				return location;
			}
			set {
				FreezableHelper.ThrowIfFrozen(this);
				location = value;
			}
		}

		public IList<IUnresolvedAttribute> AssemblyAttributes {
			get { return assemblyAttributes; }
		}
		
		IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.AssemblyAttributes {
			get { return assemblyAttributes; }
		}
		
		public IList<IUnresolvedAttribute> ModuleAttributes {
			get { return moduleAttributes; }
		}
		
		IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.ModuleAttributes {
			get { return moduleAttributes; }
		}
		
		public IEnumerable<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get { return typeDefinitions.Values; }
		}
		
		/// <summary>
		/// Adds a new top-level type definition to this assembly.
		/// </summary>
		/// <remarks>UnresolvedAssemblySpec does not support partial classes.
		/// Adding more than one part of a type will cause an ArgumentException.</remarks>
		public void AddTypeDefinition(IUnresolvedTypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			if (typeDefinition.DeclaringTypeDefinition != null)
				throw new ArgumentException("Cannot add nested types.");
			FreezableHelper.ThrowIfFrozen(this);
			var key = new TopLevelTypeName(typeDefinition.Namespace, typeDefinition.Name, typeDefinition.TypeParameters.Count);
			typeDefinitions.Add(key, typeDefinition);
		}
		
		
		/// <summary>
		/// Adds a type forwarder.
		/// This adds both an assembly attribute and an internal forwarder entry, which will be used
		/// by the resolved assembly to provide the forwarded types.
		/// </summary>
		/// <param name="typeName">The name of the type.</param>
		/// <param name="referencedType">The reference used to look up the type in the target assembly.</param>
		public void AddTypeForwarder(TopLevelTypeName typeName, ITypeReference referencedType)
		{
			if (referencedType == null)
				throw new ArgumentNullException("referencedType");
			FreezableHelper.ThrowIfFrozen(this);

			
			typeForwarders[typeName] = referencedType;
		}
		
		[Serializable]
		sealed class TypeOfConstantValue : IConstantValue
		{
			readonly ITypeReference typeRef;
			
			public TypeOfConstantValue(ITypeReference typeRef)
			{
				this.typeRef = typeRef;
			}

            public AST.Expression ResolveConstant(ITypeResolveContext context)
			{
                //return new TypeOfResolveResult(context.Compilation.FindType(KnownTypeCode.Type), typeRef.ResolveScope(context)); //TODO:Fix this
                return null;
			}
		}
		
		public IUnresolvedTypeDefinition GetTypeDefinition(string ns, string name, int typeParameterCount)
		{
			var key = new TopLevelTypeName(ns ?? string.Empty, name, typeParameterCount);
			IUnresolvedTypeDefinition td;
			if (typeDefinitions.TryGetValue(key, out td))
				return td;
			else
				return null;
		}
		
		public IAssembly Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			Freeze();
			var cache = context.Compilation.CacheManager;
			IAssembly asm = (IAssembly)cache.GetShared(this);
			if (asm != null) {
				return asm;
			} else {
				asm = new DefaultResolvedAssembly(context.Compilation, this);
				return (IAssembly)cache.GetOrAddShared(this, asm);
			}
		}
		
		public override string ToString()
		{
			return "[" + GetType().Name + " " + assemblyName + "]";
		}
		
		//[NonSerialized]
		//List<Dictionary<TopLevelTypeName, IUnresolvedTypeDefinition>> cachedTypeDictionariesPerNameComparer;
		
		Dictionary<TopLevelTypeName, IUnresolvedTypeDefinition> GetTypeDictionary(StringComparer nameComparer)
		{
			Debug.Assert(IsFrozen);
			if (nameComparer == StringComparer.Ordinal)
				return typeDefinitions;
			else
				throw new NotImplementedException();
		}
		
		#region UnresolvedNamespace
		sealed class UnresolvedNamespace
		{
			internal readonly string FullName;
			internal readonly string Name;
			internal readonly List<UnresolvedNamespace> Children = new List<UnresolvedNamespace>();
			
			public UnresolvedNamespace(string fullName, string name)
			{
				this.FullName = fullName;
				this.Name = name;
			}
		}
		
		[NonSerialized]
		List<KeyValuePair<StringComparer, UnresolvedNamespace>> unresolvedNamespacesPerNameComparer;
		
		UnresolvedNamespace GetUnresolvedRootNamespace(StringComparer nameComparer)
		{
			Debug.Assert(IsFrozen);
			LazyInitializer.EnsureInitialized(ref unresolvedNamespacesPerNameComparer);
			lock (unresolvedNamespacesPerNameComparer) {
				foreach (var pair in unresolvedNamespacesPerNameComparer) {
					if (pair.Key == nameComparer)
						return pair.Value;
				}
				var root = new UnresolvedNamespace(string.Empty, string.Empty);
				var dict = new Dictionary<string, UnresolvedNamespace>(nameComparer);
				dict.Add(root.FullName, root);
				foreach (var typeName in typeDefinitions.Keys) {
					GetOrAddNamespace(dict, typeName.Namespace);
				}
				unresolvedNamespacesPerNameComparer.Add(new KeyValuePair<StringComparer, UnresolvedNamespace>(nameComparer, root));
				return root;
			}
		}
		
		static UnresolvedNamespace GetOrAddNamespace(Dictionary<string, UnresolvedNamespace> dict, string fullName)
		{
			UnresolvedNamespace ns;
			if (dict.TryGetValue(fullName, out ns))
				return ns;
			int pos = fullName.LastIndexOf('.');
			UnresolvedNamespace parent;
			string name;
			if (pos < 0) {
				parent = dict[string.Empty]; // root
				name = fullName;
			} else {
				parent = GetOrAddNamespace(dict, fullName.Substring(0, pos));
				name = fullName.Substring(pos + 1);
			}
			ns = new UnresolvedNamespace(fullName, name);
			parent.Children.Add(ns);
			dict.Add(fullName, ns);
			return ns;
		}
		#endregion
		
		sealed class DefaultResolvedAssembly : IAssembly
		{
			readonly UnresolvedAssemblySpec unresolvedAssembly;
			readonly ICompilation compilation;
			readonly ITypeResolveContext context;
			readonly Dictionary<TopLevelTypeName, IUnresolvedTypeDefinition> unresolvedTypeDict;
			readonly ConcurrentDictionary<IUnresolvedTypeDefinition, ITypeDefinition> typeDict = new ConcurrentDictionary<IUnresolvedTypeDefinition, ITypeDefinition>();
			readonly INamespace rootNamespace;
			
			public DefaultResolvedAssembly(ICompilation compilation, UnresolvedAssemblySpec unresolved)
			{
				this.compilation = compilation;
				this.unresolvedAssembly = unresolved;
				this.unresolvedTypeDict = unresolved.GetTypeDictionary(compilation.NameComparer);
				this.rootNamespace = new NS(this, unresolved.GetUnresolvedRootNamespace(compilation.NameComparer), null);
				this.context = new SimpleTypeResolveContext(this);
				this.AssemblyAttributes = unresolved.AssemblyAttributes.CreateResolvedAttributes(context);
				this.ModuleAttributes = unresolved.ModuleAttributes.CreateResolvedAttributes(context);
			}
			
			public IUnresolvedAssembly UnresolvedAssembly {
				get { return unresolvedAssembly; }
			}
			
			public bool IsMainAssembly {
				get { return this.Compilation.MainAssembly == this; }
			}
			
			public string AssemblyName {
				get { return unresolvedAssembly.AssemblyName; }
			}
			
			public string FullAssemblyName {
				get { return unresolvedAssembly.FullAssemblyName; }
			}
			
			public IList<IAttribute> AssemblyAttributes { get; private set; }
			public IList<IAttribute> ModuleAttributes { get; private set; }
			
			public INamespace RootNamespace {
				get { return rootNamespace; }
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
							&& attr.AttributeType.Namespace == "Std.Runtime"
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

			public ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName)
			{
				IUnresolvedTypeDefinition td;
				ITypeReference typeRef;
				if (unresolvedAssembly.typeDefinitions.TryGetValue(topLevelTypeName, out td))
					return GetTypeDefinition(td);
				if (unresolvedAssembly.typeForwarders.TryGetValue(topLevelTypeName, out typeRef)) {
					// Protect against cyclic type forwarders:
					using (var busyLock = BusyManager.Enter(typeRef)) {
						if (busyLock.Success)
							return typeRef.Resolve(compilation.TypeResolveContext).GetDefinition();
					}
				}
				return null;
			}
			
			ITypeDefinition GetTypeDefinition(IUnresolvedTypeDefinition unresolved)
			{
				return typeDict.GetOrAdd(unresolved, t => CreateTypeDefinition(t));
			}
			
			ITypeDefinition CreateTypeDefinition(IUnresolvedTypeDefinition unresolved)
			{
				if (unresolved.DeclaringTypeDefinition != null) {
					ITypeDefinition declaringType = GetTypeDefinition(unresolved.DeclaringTypeDefinition);
					return new ResolvedTypeDefinitionSpec(context.WithCurrentTypeDefinition(declaringType), unresolved);
                }
                else if (unresolved.Name == "Void" && unresolved.Namespace == "Std" && unresolved.TypeParameters.Count == 0)
                {
					return new VoidTypeDefinition(context, unresolved);
				} else {
					return new ResolvedTypeDefinitionSpec(context, unresolved);
				}
			}
			
			public IEnumerable<ITypeDefinition> TopLevelTypeDefinitions {
				get {
					return unresolvedAssembly.TopLevelTypeDefinitions.Select(t => GetTypeDefinition(t));
				}
			}
			
			public override string ToString()
			{
				return "[DefaultResolvedAssembly " + AssemblyName + "]";
			}
			
			sealed class NS : INamespace
			{
				readonly DefaultResolvedAssembly assembly;
				readonly UnresolvedNamespace ns;
				readonly INamespace parentNamespace;
				readonly IList<NS> childNamespaces;
				IEnumerable<ITypeDefinition> types;
				
				public NS(DefaultResolvedAssembly assembly, UnresolvedNamespace ns, INamespace parentNamespace)
				{
					this.assembly = assembly;
					this.ns = ns;
					this.parentNamespace = parentNamespace;
					this.childNamespaces = new ProjectedList<NS, UnresolvedNamespace, NS>(
						this, ns.Children, (self, c) => new NS(self.assembly, c, self));
				}
				
				string INamespace.ExternAlias {
					get { return null; }
				}
				
				string INamespace.FullName {
					get { return ns.FullName; }
				}
				
				SymbolKind ISymbol.SymbolKind {
					get { return SymbolKind.Namespace; }
				}
				
				public string Name {
					get { return ns.Name; }
				}
				
				INamespace INamespace.ParentNamespace {
					get { return parentNamespace; }
				}
				
				IEnumerable<IAssembly> INamespace.ContributingAssemblies {
					get { return new [] { assembly }; }
				}
				
				IEnumerable<INamespace> INamespace.ChildNamespaces {
					get { return childNamespaces; }
				}
				
				INamespace INamespace.GetChildNamespace(string name)
				{
					var nameComparer = assembly.compilation.NameComparer;
					for (int i = 0; i < childNamespaces.Count; i++) {
						if (nameComparer.Equals(name, ns.Children[i].Name))
							return childNamespaces[i];
					}
					return null;
				}
				
				ICompilation ICompilationProvider.Compilation {
					get { return assembly.compilation; }
				}
				
				IEnumerable<ITypeDefinition> INamespace.Types {
					get {
						var result = LazyInit.VolatileRead(ref this.types);
						if (result != null) {
							return result;
						} else {
							var hashSet = new HashSet<ITypeDefinition>();
							foreach (IUnresolvedTypeDefinition typeDef in assembly.UnresolvedAssembly.TopLevelTypeDefinitions) {
								if (typeDef.Namespace == ns.FullName)
									hashSet.Add(assembly.GetTypeDefinition(typeDef));
							}
							return LazyInit.GetOrSet(ref this.types, hashSet.ToArray());
						}
					}
				}
				
				ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
				{
					var key = new TopLevelTypeName(ns.FullName, name, typeParameterCount);
					IUnresolvedTypeDefinition unresolvedTypeDef;
					if (assembly.unresolvedTypeDict.TryGetValue(key, out unresolvedTypeDef))
						return assembly.GetTypeDefinition(unresolvedTypeDef);
					else
						return null;
				}
				
				public ISymbolReference ToReference()
				{
					return new NamespaceReference(new AssemblyReferenceSpec(assembly.AssemblyName), ns.FullName);
				}
			}
		}
	}
}
