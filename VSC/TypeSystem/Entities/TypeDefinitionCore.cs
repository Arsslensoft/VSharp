using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VSC.AST;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Represents an unresolved type definition.
	/// </summary>
	[Serializable]
	public class TypeDefinitionCore : EntityCore, IUnresolvedTypeDefinition
	{
		TypeKind kind = TypeKind.Class;
		string namespaceName;
		IList<ITypeReference> baseTypes;
		public  IList<IUnresolvedTypeParameter> typeParameters;
		IList<IUnresolvedTypeDefinition> nestedTypes;
		IList<IUnresolvedMember> members;
		
		public TypeDefinitionCore()
		{
			this.SymbolKind = SymbolKind.TypeDefinition;
		}
		
		public TypeDefinitionCore(string fullName)
		{
			string namespaceName;
			string name;
			int idx = fullName.LastIndexOf ('.');
			if (idx > 0) {
				namespaceName = fullName.Substring (0, idx);
				name = fullName.Substring (idx + 1);
			} else {
				namespaceName = "";
				name = fullName;
			}

			this.SymbolKind = SymbolKind.TypeDefinition;
			this.namespaceName = namespaceName;
			this.Name = name;
		}
		
		public TypeDefinitionCore(string namespaceName, string name)
		{
			this.SymbolKind = SymbolKind.TypeDefinition;
			this.namespaceName = namespaceName;
			this.Name = name;
		}
		
		public TypeDefinitionCore(IUnresolvedTypeDefinition declaringTypeDefinition, string name)
		{
		    if (declaringTypeDefinition.Name != "default")
		    {
		        this.SymbolKind = SymbolKind.TypeDefinition;
		        this.DeclaringTypeDefinition = declaringTypeDefinition;
		        this.namespaceName = declaringTypeDefinition.Namespace;
		        this.Name = name;
		        this.UnresolvedFile = declaringTypeDefinition.UnresolvedFile;
		    }
		    else
		    {
                this.SymbolKind = SymbolKind.TypeDefinition;
                this.namespaceName = declaringTypeDefinition.Namespace;
                this.Name = name;
		    }
		}
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
			baseTypes = FreezableHelper.FreezeList(baseTypes);
			typeParameters = FreezableHelper.FreezeListAndElements(typeParameters);
			nestedTypes = FreezableHelper.FreezeListAndElements(nestedTypes);
			members = FreezableHelper.FreezeListAndElements(members);
		}
		
		public override object Clone()
		{
			var copy = (TypeDefinitionCore)base.Clone();
			if (baseTypes != null)
				copy.baseTypes = new List<ITypeReference>(baseTypes);
			if (typeParameters != null)
				copy.typeParameters = new List<IUnresolvedTypeParameter>(typeParameters);
			if (nestedTypes != null)
				copy.nestedTypes = new List<IUnresolvedTypeDefinition>(nestedTypes);
			if (members != null)
				copy.members = new List<IUnresolvedMember>(members);
			return copy;
		}
		
		public TypeKind Kind {
			get { return kind; }
			set {
				ThrowIfFrozen();
				kind = value;
			}
		}
		
		public bool AddDefaultConstructorIfRequired {
			get { return flags[FlagAddDefaultConstructorIfRequired]; }
			set {
				ThrowIfFrozen();
				flags[FlagAddDefaultConstructorIfRequired] = value;
			}
		}
		
		public bool? HasExtensionMethods {
			get {
				if (flags[FlagHasExtensionMethods])
					return true;
				else if (flags[FlagHasNoExtensionMethods])
					return false;
				else
					return null;
			}
			set {
				ThrowIfFrozen();
				flags[FlagHasExtensionMethods] = (value == true);
				flags[FlagHasNoExtensionMethods] = (value == false);
			}
		}

        public bool IsPartial
        {
            get { return flags[FlagPartialTypeDefinition]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagPartialTypeDefinition] = value;
            }
        }
		
		public override string Namespace {
			get { return namespaceName; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				ThrowIfFrozen();
				namespaceName = value;
			}
		}
		
		public override string ReflectionName {
			get {
				return this.FullTypeName.ReflectionName;
			}
		}
		
		public FullTypeName FullTypeName {
			get {
				IUnresolvedTypeDefinition declaringTypeDef = this.DeclaringTypeDefinition;
				if (declaringTypeDef != null) {
					return declaringTypeDef.FullTypeName.NestedType(this.Name, this.TypeParameters.Count - declaringTypeDef.TypeParameters.Count);
				} else {
					return new TopLevelTypeName(namespaceName, this.Name, this.TypeParameters.Count);
				}
			}
		}
		
		public IList<ITypeReference> BaseTypes {
			get {
				if (baseTypes == null)
					baseTypes = new List<ITypeReference>();
				return baseTypes;
			}
		}
		
		public IList<IUnresolvedTypeParameter> TypeParameters {
			get {
				if (typeParameters == null)
					typeParameters = new List<IUnresolvedTypeParameter>();
				return typeParameters;
			}
		}



	    public IList<IUnresolvedTypeDefinition> NestedTypes {
			get {
				if (nestedTypes == null)
					nestedTypes = new List<IUnresolvedTypeDefinition>();
				return nestedTypes;
			}
		}
		
		public IList<IUnresolvedMember> Members {
			get {
				if (members == null)
					members = new List<IUnresolvedMember>();
				return members;
			}
		}
		
		public IEnumerable<IUnresolvedMethod> Methods {
			get {
				return Members.OfType<IUnresolvedMethod> ();
			}
		}
		
		public IEnumerable<IUnresolvedProperty> Properties {
			get {
				return Members.OfType<IUnresolvedProperty> ();
			}
		}
		
		public IEnumerable<IUnresolvedField> Fields {
			get {
				return Members.OfType<IUnresolvedField> ();
			}
		}
		
		public IEnumerable<IUnresolvedEvent> Events {
			get {
				return Members.OfType<IUnresolvedEvent> ();
			}
		}
		
		
		public IType Resolve(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (context.CurrentAssembly == null)
				throw new ArgumentException("An ITypeDefinition cannot be resolved in a context without a current assembly.");
			return context.CurrentAssembly.GetTypeDefinition(this.FullTypeName)
				?? (IType)new UnknownTypeSpec(this.Namespace, this.Name, this.TypeParameters.Count);
		}
		
		public virtual ITypeResolveContext CreateResolveContext(ITypeResolveContext parentContext)
		{
			return parentContext;
		}


        public override string GetSignatureForDocumentation()
        {
            throw new NotImplementedException();
        }
	}
}
