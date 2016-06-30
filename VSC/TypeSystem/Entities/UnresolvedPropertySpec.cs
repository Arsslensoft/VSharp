using System;
using System.Collections.Generic;
using System.Linq;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IUnresolvedProperty"/>.
	/// </summary>
	[Serializable]
	public class UnresolvedPropertySpec : UnresolvedMemberSpec, IUnresolvedProperty
	{
		IUnresolvedMethod getter, setter;
		IList<IUnresolvedParameter> parameters;
		
		protected override void FreezeInternal()
		{
			parameters = FreezableHelper.FreezeListAndElements(parameters);
			FreezableHelper.Freeze(getter);
			FreezableHelper.Freeze(setter);
			base.FreezeInternal();
		}
		
		public override object Clone()
		{
			var copy = (UnresolvedPropertySpec)base.Clone();
			if (parameters != null)
				copy.parameters = new List<IUnresolvedParameter>(parameters);
			return copy;
		}
		
		public override void ApplyInterningProvider(InterningProvider provider)
		{
			base.ApplyInterningProvider(provider);
			parameters = provider.InternList(parameters);
		}
		
		public UnresolvedPropertySpec()
		{
			this.SymbolKind = SymbolKind.Property;
		}
		
		public UnresolvedPropertySpec(IUnresolvedTypeDefinition declaringType, string name)
		{
			this.SymbolKind = SymbolKind.Property;
			this.DeclaringTypeDefinition = declaringType;
			this.Name = name;
			if (declaringType != null)
				this.UnresolvedFile = declaringType.UnresolvedFile;
		}
		
		public bool IsIndexer {
			get { return this.SymbolKind == SymbolKind.Indexer; }
		}
		
		public IList<IUnresolvedParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IUnresolvedParameter>();
				return parameters;
			}
		}
		
		public bool CanGet {
			get { return getter != null; }
		}
		
		public bool CanSet {
			get { return setter != null; }
		}
		
		public IUnresolvedMethod Getter {
			get { return getter; }
			set {
				ThrowIfFrozen();
				getter = value;
			}
		}
		
		public IUnresolvedMethod Setter {
			get { return setter; }
			set {
				ThrowIfFrozen();
				setter = value;
			}
		}
		
		public override IMember CreateResolved(ITypeResolveContext context)
		{
			return new ResolvedPropertySpec(this, context);
		}
		
		public override IMember Resolve(ITypeResolveContext context)
		{
			ITypeReference interfaceTypeReference = null;
			if (this.IsExplicitInterfaceImplementation && this.ExplicitInterfaceImplementations.Count == 1)
				interfaceTypeReference = this.ExplicitInterfaceImplementations[0].DeclaringTypeReference;
			return Resolve(ExtendContextForType(context, this.DeclaringTypeDefinition), 
			               this.SymbolKind, this.Name, interfaceTypeReference,
			               parameterTypeReferences: this.Parameters.Select(p => p.Type).ToList());
		}
		
		IProperty IUnresolvedProperty.Resolve(ITypeResolveContext context)
		{
			return (IProperty)Resolve(context);
		}
	}
}
