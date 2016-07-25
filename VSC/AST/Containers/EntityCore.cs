using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
	/// <summary>
	/// Base class for <see cref="IUnresolvedEntity"/> implementations.
	/// </summary>
	[Serializable]
    public abstract class EntityCore : IUnresolvedEntity, IFreezable, IModuleSupport
	{

	
		// possible optimizations to reduce the memory usage of EntityCore:
		// - store regions in more compact form (e.g. assume both file names are identical; use ushort for columns)
		
		protected IUnresolvedTypeDefinition declaringTypeDefinition;

      protected  Modifiers mod_flags = Modifiers.NONE;
		protected string name = string.Empty;
		protected IList<IUnresolvedAttribute> attributes;
		internal RareFields rareFields;
		
		// 1 byte per enum + 2 bytes for flags
		SymbolKind symbolKind;
		internal BitVector16 flags;
		
		// flags for EntityCore:
		internal const ushort FlagFrozen    = 0x0001;
		// flags for TypeDefinitionCore/LazyCecilTypeDefinition
		internal const ushort FlagAddDefaultConstructorIfRequired = 0x0040;
		internal const ushort FlagHasExtensionMethods = 0x0080;
		internal const ushort FlagHasNoExtensionMethods = 0x0100;
        internal const ushort FlagPartialTypeDefinition = 0x0200;
		// flags for MemberContainer:
		internal const ushort FlagExplicitInterfaceImplementation = 0x0040;
		// flags for DefaultMethod:
		internal const ushort FlagExtensionMethod = 0x1000;
		internal const ushort FlagHasBody = 0x4000;

		public bool IsFrozen {
			get { return flags[FlagFrozen]; }
		}
		
		public void Freeze()
		{
			if (!flags[FlagFrozen]) {
				FreezeInternal();
				flags[FlagFrozen] = true;
			}
		}
		
		protected virtual void FreezeInternal()
		{
			attributes = FreezableHelper.FreezeListAndElements(attributes);
			if (rareFields != null)
				rareFields.FreezeInternal();
		}
		
		/// <summary>
		/// Uses the specified interning provider to intern
		/// strings and lists in this entity.
		/// This method does not test arbitrary objects to see if they implement ISupportsInterning;
		/// instead we assume that those are interned immediately when they are created (before they are added to this entity).
		/// </summary>
		public virtual void ApplyInterningProvider(InterningProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			ThrowIfFrozen();
			name = provider.Intern(name);
			attributes = provider.InternList(attributes);
			if (rareFields != null)
				rareFields.ApplyInterningProvider(provider);
		}
		
		/// <summary>
		/// Creates a shallow clone of this entity.
		/// Collections (e.g. a type's member list) will be cloned as well, but the elements
		/// of said list will not be.
		/// If this instance is frozen, the clone will be unfrozen.
		/// </summary>
		public virtual object Clone()
		{
			var copy = (EntityCore)MemberwiseClone();
			copy.flags[FlagFrozen] = false;
			if (attributes != null)
				copy.attributes = new List<IUnresolvedAttribute>(attributes);
			if (rareFields != null)
				copy.rareFields = (RareFields)rareFields.Clone();
			return copy;
		}
		
		[Serializable]
		internal class RareFields
		{
			internal DomRegion region;
			internal DomRegion bodyRegion;
			internal IUnresolvedFile unresolvedFile;
			
			protected internal virtual void FreezeInternal()
			{
			}
			
			public virtual void ApplyInterningProvider(InterningProvider provider)
			{
			}
			
			public virtual object Clone()
			{
				return MemberwiseClone();
			}
		}
		
		protected void ThrowIfFrozen()
		{
			FreezableHelper.ThrowIfFrozen(this);
		}
		
		public SymbolKind SymbolKind {
			get { return symbolKind; }
			set {
				ThrowIfFrozen();
				symbolKind = value;
			}
		}
        public Modifiers ModFlags
        {
            get { return mod_flags; }
            set
            {
                ThrowIfFrozen();
                mod_flags = value;
            }
        }
        protected MemberName member_name;
        public MemberName MemberName
        {
            get { return member_name; }
        }
        /// <summary>
        ///   Location where this declaration happens
        /// </summary>
        public virtual Location Location
        {
            get { return member_name.Location; }
        }

        /// <summary>
        /// Parent container
        /// </summary>
        public TypeContainer Parent;
        string comment = "";
        public string DocComment
        {
            get
            {
                return comment;
            }
            set
            {
                if (value == null)
                    return;

                comment += value;
            }
        }
        //
        // Returns a string that represents the signature for this 
        // member which should be used in XML documentation.
        //
        public abstract string GetSignatureForDocumentation();
        public bool IsCompilerGenerated
        {
            get
            {
                if ((mod_flags & Modifiers.COMPILER_GENERATED) != 0)
                    return true;

                return Parent != null && Parent.IsCompilerGenerated;
            }
        }

        protected Report Report
        {
            get
            {
                return Compiler.Report;
            }
        }
        public virtual CompilerContext Compiler
        {
            get
            {
                return Module.Compiler;
            }
        }

	    private ModuleContext module;
        public virtual ModuleContext Module
        {
            get
            {
                if (Parent != null)
                    return Parent.Module;
                else return module;
            }
            set { module = value; }
        }
        protected virtual void CheckProtected(ResolveContext rc)
        {
            if (Accessibility != Accessibility.Protected)
                return;

            if (Parent.Kind == TypeKind.Struct)
            {
                rc.Report.Error(199, Location, "`{0}': Structs cannot contain protected members",
                    GetSignatureForError());
                return;
            }

            if (Parent.IsStatic)
            {
                rc.Report.Error(200, Location, "`{0}': Static classes cannot contain protected members",
                    GetSignatureForError());
                return;
            }

            if (Parent.IsSealed &&  (mod_flags & Modifiers.OVERRIDE) == 0 &&
                !(this is DestructorDeclaration))
            {
                rc.Report.Warning(201, 4, Location, "`{0}': new protected member declared in sealed class",
                    GetSignatureForError());
                return;
            }
        }
        protected virtual void CheckAbstractExtern(ResolveContext rc)
        {
            if (Parent.Kind == TypeKind.Interface)
                return;

            if (this is MethodCore && (this as MethodCore).HasBody)
            {
                if ((mod_flags & Modifiers.EXTERN) != 0)
                {
                    rc.Report.Error(202, Location, "`{0}' cannot declare a body because it is marked extern",
                        GetSignatureForError());

                }

                if ((mod_flags & Modifiers.ABSTRACT) != 0)
                {
                    rc.Report.Error(203, Location, "`{0}' cannot declare a body because it is marked abstract",
                        GetSignatureForError());

                }
            }
            else
            {
                if ((mod_flags & (Modifiers.ABSTRACT | Modifiers.EXTERN | Modifiers.PARTIAL)) == 0 && !(Parent is DelegateDeclaration))
                {

                    PropertyMethod pm = this as PropertyMethod;
                    if (pm is IndexerGetterDeclaration || pm is IndexerSetterDeclaration)
                        pm = null;

                    if (pm != null && pm.Property.AccessorSecond == null)
                        rc.Report.Error(204, Location,
                            "`{0}' must have a body because it is not marked abstract or extern. The property can be automatically implemented when you define both accessors",
                            GetSignatureForError());


                    rc.Report.Error(205, Location, "`{0}' must have a body because it is not marked abstract, extern, or partial",
                                  GetSignatureForError());

                }
            }
        }

       // public abstract string GetSignatureForDocumentation(); //TODO:Add DOC
        public virtual string GetSignatureForError()
        {
            if (Parent != null)
            {
                var parent = Parent.GetSignatureForError();
                if (parent == null)
                    return member_name.GetSignatureForError();

                return parent + "." + member_name.GetSignatureForError();

            }
            else
                return member_name.GetSignatureForError();


        }

		internal virtual RareFields WriteRareFields()
		{
			ThrowIfFrozen();
			if (rareFields == null) rareFields = new RareFields();
			return rareFields;
		}
		
		public DomRegion Region {
			get { return rareFields != null ? rareFields.region : DomRegion.Empty; }
			set {
				if (value != DomRegion.Empty || rareFields != null)
					WriteRareFields().region = value;
			}
		}
		
		public DomRegion BodyRegion {
			get { return rareFields != null ? rareFields.bodyRegion : DomRegion.Empty; }
			set {
				if (value != DomRegion.Empty || rareFields != null)
					WriteRareFields().bodyRegion = value;
			}
		}
		
		public IUnresolvedFile UnresolvedFile {
			get { return rareFields != null ? rareFields.unresolvedFile : null; }
			set {
				if (value != null || rareFields != null)
					WriteRareFields().unresolvedFile = value;
			}
		}
		
		public IUnresolvedTypeDefinition DeclaringTypeDefinition {
			get { return declaringTypeDefinition; }
			set {
				ThrowIfFrozen();
				declaringTypeDefinition = value;
			}
		}

	 
		public IList<IUnresolvedAttribute> Attributes {
			get {
				if (attributes == null)
					attributes = new List<IUnresolvedAttribute>();
				return attributes;
			}
		}
		
		public string Name {
			get { return name; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				ThrowIfFrozen();
				name = value;
			}
		}
		
		public virtual string FullName {
			get {
				if (declaringTypeDefinition != null)
					return declaringTypeDefinition.FullName + "." + name;
				else if (!string.IsNullOrEmpty(this.Namespace))
					return this.Namespace + "." + name;
				else
					return name;
			}
		}
		
		public virtual string Namespace {
			get {
				if (declaringTypeDefinition != null)
					return declaringTypeDefinition.Namespace;
				else
					return string.Empty;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public virtual string ReflectionName {
			get {
				if (declaringTypeDefinition != null)
					return declaringTypeDefinition.ReflectionName + "." + name;
				else
					return name;
			}
		}
		
		public Accessibility Accessibility {
		    get
		    {
	
                switch (mod_flags & VSC.TypeSystem.Modifiers.AccessibilityMask)
                {
                    case VSC.TypeSystem.Modifiers.PRIVATE:
                        return Accessibility.Private;
                    case VSC.TypeSystem.Modifiers.INTERNAL:
                        return Accessibility.Internal;
                    case VSC.TypeSystem.Modifiers.PROTECTED | VSC.TypeSystem.Modifiers.INTERNAL:
                        return Accessibility.ProtectedOrInternal;
                    case VSC.TypeSystem.Modifiers.PROTECTED:
                        return Accessibility.Protected;
                    case VSC.TypeSystem.Modifiers.PUBLIC:
                        return Accessibility.Public;
                    default:
                        return Accessibility.None;
                }
		    }
		}
		
		public bool IsStatic {
            get { return (mod_flags & Modifiers.STATIC) != 0; }
		
		}
		
		public bool IsAbstract {
            get { return (mod_flags & Modifiers.ABSTRACT) != 0; }
		}
		
		public bool IsSealed {
			get { return (mod_flags & Modifiers.SEALED) != 0; }
		}
		
		public bool IsShadowing {
            get { return (mod_flags & Modifiers.NEW) != 0; }
		}
		
		public bool IsSynthetic {
            get { return (mod_flags & Modifiers.COMPILER_GENERATED) != 0; }
		}
		
		bool IHasAccessibility.IsPrivate {
			get { return Accessibility == Accessibility.Private; }
		}
		
		bool IHasAccessibility.IsPublic {
            get { return Accessibility == Accessibility.Public; }
		}
		
		bool IHasAccessibility.IsProtected {
            get { return Accessibility == Accessibility.Protected; }
		}
		
		bool IHasAccessibility.IsInternal {
            get { return Accessibility == Accessibility.Internal; }
		}
		
		bool IHasAccessibility.IsProtectedOrInternal {
            get { return Accessibility == Accessibility.ProtectedOrInternal; }
		}
		
	
		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(GetType().Name);
			b.Append(' ');
			if (this.DeclaringTypeDefinition != null) {
				b.Append(this.DeclaringTypeDefinition.Name);
				b.Append('.');
			}
			b.Append(this.Name);
			b.Append(']');
			return b.ToString();
		}
   
	
	}
}
