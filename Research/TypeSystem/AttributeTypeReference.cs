using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;


namespace VSC.TypeSystem
{
	/// <summary>
	/// Type reference used within an attribute.
	/// Looks up both 'withoutSuffix' and 'withSuffix' and returns the type that exists.
	/// </summary>
	[Serializable]
	public sealed class AttributeTypeReference : ITypeReference, ISupportsInterning
	{
		readonly ITypeReference withoutSuffix, withSuffix;
		
		public AttributeTypeReference(ITypeReference withoutSuffix, ITypeReference withSuffix)
		{
			if (withoutSuffix == null)
				throw new ArgumentNullException("withoutSuffix");
			if (withSuffix == null)
				throw new ArgumentNullException("withSuffix");
			this.withoutSuffix = withoutSuffix;
			this.withSuffix = withSuffix;
		}
		
		public IType Resolve(ITypeResolveContext context)
		{
			IType t1 = withoutSuffix.Resolve(context);
			IType t2 = withSuffix.Resolve(context);
			return PreferAttributeTypeWithSuffix(t1, t2, context.Compilation) ? t2 : t1;
		}
		
		internal static bool PreferAttributeTypeWithSuffix(IType t1, IType t2, ICompilation compilation)
		{
			if (t2.Kind == TypeKind.Unknown) return false;
			if (t1.Kind == TypeKind.Unknown) return true;
			
			var attrTypeDef = compilation.FindType(KnownTypeCode.Attribute).GetDefinition();
			if (attrTypeDef != null) {
				bool t1IsAttribute = (t1.GetDefinition() != null && t1.GetDefinition().IsDerivedFrom(attrTypeDef));
				bool t2IsAttribute = (t2.GetDefinition() != null && t2.GetDefinition().IsDerivedFrom(attrTypeDef));
				if (t2IsAttribute && !t1IsAttribute)
					return true;
				// If both types exist and are attributes, C# considers that to be an ambiguity, but we are less strict.
			}
			return false;
		}
		
		public override string ToString()
		{
			return withoutSuffix.ToString() + "[Attribute]";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			unchecked {
				return withoutSuffix.GetHashCode() + 715613 * withSuffix.GetHashCode();
			}
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			AttributeTypeReference atr = other as AttributeTypeReference;
			return atr != null && this.withoutSuffix == atr.withoutSuffix && this.withSuffix == atr.withSuffix;
		}
	}
}
