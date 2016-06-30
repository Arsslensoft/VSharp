using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IUnresolvedMethod" /> interface.
	/// </summary>
	[Serializable]
	public class UnresolvedMethodSpec : UnresolvedMemberSpec, IUnresolvedMethod
	{
		IList<IUnresolvedAttribute> returnTypeAttributes;
		IList<IUnresolvedTypeParameter> typeParameters;
		IList<IUnresolvedParameter> parameters;
		IUnresolvedMember accessorOwner;
		
		protected override void FreezeInternal()
		{
			returnTypeAttributes = FreezableHelper.FreezeListAndElements(returnTypeAttributes);
			typeParameters = FreezableHelper.FreezeListAndElements(typeParameters);
			parameters = FreezableHelper.FreezeListAndElements(parameters);
			base.FreezeInternal();
		}
		
		public override object Clone()
		{
			var copy = (UnresolvedMethodSpec)base.Clone();
			if (returnTypeAttributes != null)
				copy.returnTypeAttributes = new List<IUnresolvedAttribute>(returnTypeAttributes);
			if (typeParameters != null)
				copy.typeParameters = new List<IUnresolvedTypeParameter>(typeParameters);
			if (parameters != null)
				copy.parameters = new List<IUnresolvedParameter>(parameters);
			return copy;
		}
		
		public override void ApplyInterningProvider(InterningProvider provider)
		{
			base.ApplyInterningProvider(provider);
			if (provider != null) {
				returnTypeAttributes = provider.InternList(returnTypeAttributes);
				typeParameters = provider.InternList(typeParameters);
				parameters = provider.InternList(parameters);
			}
		}
		
		public UnresolvedMethodSpec()
		{
			this.SymbolKind = SymbolKind.Method;
		}
		
		public UnresolvedMethodSpec(IUnresolvedTypeDefinition declaringType, string name)
		{
			this.SymbolKind = SymbolKind.Method;
			this.DeclaringTypeDefinition = declaringType;
			this.Name = name;
			if (declaringType != null)
				this.UnresolvedFile = declaringType.UnresolvedFile;
		}
		
		public IList<IUnresolvedAttribute> ReturnTypeAttributes {
			get {
				if (returnTypeAttributes == null)
					returnTypeAttributes = new List<IUnresolvedAttribute>();
				return returnTypeAttributes;
			}
		}
		
		public IList<IUnresolvedTypeParameter> TypeParameters {
			get {
				if (typeParameters == null)
					typeParameters = new List<IUnresolvedTypeParameter>();
				return typeParameters;
			}
		}
		
		public bool IsExtensionMethod {
			get { return flags[FlagExtensionMethod]; }
			set {
				ThrowIfFrozen();
				flags[FlagExtensionMethod] = value;
			}
		}
		
		public bool IsConstructor {
			get { return this.SymbolKind == SymbolKind.Constructor; }
		}
		
		public bool IsDestructor {
			get { return this.SymbolKind == SymbolKind.Destructor; }
		}
		
		public bool IsOperator {
			get { return this.SymbolKind == SymbolKind.Operator; }
		}

        public bool IsSupersede
        {
            get { return flags[FlagSupersededMethod]; }
            set
            {
                ThrowIfFrozen();
                flags[FlagSupersededMethod] = value;
            }
        }
		public bool IsAsync {
			get { return flags[FlagAsyncMethod]; }
			set {
				ThrowIfFrozen();
				flags[FlagAsyncMethod] = value;
			}
		}

		public bool HasBody {
			get { return flags[FlagHasBody]; }
			set {
				ThrowIfFrozen();
				flags[FlagHasBody] = value;
			}
		}
		
		
		
		
		public IList<IUnresolvedParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IUnresolvedParameter>();
				return parameters;
			}
		}
		
		public IUnresolvedMember AccessorOwner {
			get { return accessorOwner; }
			set {
				ThrowIfFrozen();
				accessorOwner = value;
			}
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(SymbolKind.ToString());
			b.Append(' ');
			if (DeclaringTypeDefinition != null) {
				b.Append(DeclaringTypeDefinition.Name);
				b.Append('.');
			}
			b.Append(Name);
			b.Append('(');
			b.Append(string.Join(", ", this.Parameters));
			b.Append("):");
			b.Append(ReturnType.ToString());
			b.Append(']');
			return b.ToString();
		}
		
		public override IMember CreateResolved(ITypeResolveContext context)
		{
			return new ResolvedMethodSpec(this, context);
		}
		
		public override IMember Resolve(ITypeResolveContext context)
		{
			if (accessorOwner != null) {
				var owner = accessorOwner.Resolve(context);
				if (owner != null) {
					IProperty p = owner as IProperty;
					if (p != null) {
						if (p.CanGet && p.Getter.Name == this.Name)
							return p.Getter;
						if (p.CanSet && p.Setter.Name == this.Name)
							return p.Setter;
					}
					IEvent e = owner as IEvent;
					if (e != null) {
						if (e.CanAdd && e.AddAccessor.Name == this.Name)
							return e.AddAccessor;
						if (e.CanRemove && e.RemoveAccessor.Name == this.Name)
							return e.RemoveAccessor;
						if (e.CanInvoke && e.InvokeAccessor.Name == this.Name)
							return e.InvokeAccessor;
					}
				}
				return null;
			}
			
			ITypeReference interfaceTypeReference = null;
			if (this.IsExplicitInterfaceImplementation && this.ExplicitInterfaceImplementations.Count == 1)
				interfaceTypeReference = this.ExplicitInterfaceImplementations[0].DeclaringTypeReference;
			return Resolve(ExtendContextForType(context, this.DeclaringTypeDefinition),
			               this.SymbolKind, this.Name, interfaceTypeReference,
			               this.TypeParameters.Select(tp => tp.Name).ToList(),
			               this.Parameters.Select(p => p.Type).ToList());
		}
		
		IMethod IUnresolvedMethod.Resolve(ITypeResolveContext context)
		{
			return (IMethod)Resolve(context);
		}
		
		public static UnresolvedMethodSpec CreateDefaultConstructor(IUnresolvedTypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			DomRegion region = typeDefinition.Region;
			region = new DomRegion(region.FileName, region.BeginLine, region.BeginColumn); // remove endline/endcolumn
			return new UnresolvedMethodSpec(typeDefinition, ".ctor") {
				SymbolKind = SymbolKind.Constructor,
				Accessibility = typeDefinition.IsAbstract ? Accessibility.Protected : Accessibility.Public,
				IsSynthetic = true,
				HasBody = true,
				Region = region,
				BodyRegion = region,
				ReturnType = KnownTypeReference.Void
			};
		}
		
		static readonly IUnresolvedMethod dummyConstructor = CreateDummyConstructor();
		
		/// <summary>
		/// Returns a dummy constructor instance:
		/// </summary>
		/// <returns>
		/// A public instance constructor with IsSynthetic=true and no declaring type.
		/// </returns>
		public static IUnresolvedMethod DummyConstructor {
			get { return dummyConstructor; }
		}
		
		static IUnresolvedMethod CreateDummyConstructor()
		{
			var m = new UnresolvedMethodSpec {
				SymbolKind = SymbolKind.Constructor,
				Name = ".ctor",
				Accessibility = Accessibility.Public,
				IsSynthetic = true,
				ReturnType = KnownTypeReference.Void
			};
			m.Freeze();
			return m;
		}
	}
}
