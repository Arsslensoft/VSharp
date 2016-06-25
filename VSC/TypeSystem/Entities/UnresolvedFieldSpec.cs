using System;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IUnresolvedField"/>.
	/// </summary>
	[Serializable]
	public class UnresolvedFieldSpec : UnresolvedMemberSpec, IUnresolvedField
	{
		IConstantValue constantValue;
		
		protected override void FreezeInternal()
		{
			FreezableHelper.Freeze(constantValue);
			base.FreezeInternal();
		}
		
		public UnresolvedFieldSpec()
		{
			this.SymbolKind = SymbolKind.Field;
		}
		
		public UnresolvedFieldSpec(IUnresolvedTypeDefinition declaringType, string name)
		{
			this.SymbolKind = SymbolKind.Field;
			this.DeclaringTypeDefinition = declaringType;
			this.Name = name;
			if (declaringType != null)
				this.UnresolvedFile = declaringType.UnresolvedFile;
		}
		
		public bool IsConst {
			get { return constantValue != null && !IsFixed; }
		}
		
		public bool IsReadOnly {
			get { return flags[FlagFieldIsReadOnly]; }
			set {
				ThrowIfFrozen();
				flags[FlagFieldIsReadOnly] = value;
			}
		}
		
		public bool IsVolatile {
			get { return flags[FlagFieldIsVolatile]; }
			set {
				ThrowIfFrozen();
				flags[FlagFieldIsVolatile] = value;
			}
		}

		public bool IsFixed {
			get { return flags[FlagFieldIsFixedSize]; }
			set {
				ThrowIfFrozen();
				flags[FlagFieldIsFixedSize] = value;
			}
		}
		
		public IConstantValue ConstantValue {
			get { return constantValue; }
			set {
				ThrowIfFrozen();
				constantValue = value;
			}
		}
		
		public override IMember CreateResolved(ITypeResolveContext context)
		{
			return new ResolvedFieldSpec(this, context);
		}
		
		IField IUnresolvedField.Resolve(ITypeResolveContext context)
		{
			return (IField)Resolve(context);
		}
	}
}
