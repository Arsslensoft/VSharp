using System;
using VSC.Base;
using VSC.TypeSystem.Resolver;


namespace VSC.TypeSystem
{
	/// <summary>
	/// Looks up an alias (identifier in front of :: operator).
	/// </summary>
	/// <remarks>
	/// The member lookup performed by the :: operator is handled
	/// by <see cref="MemberTypeOrNamespaceReference"/>.
	/// </remarks>
	[Serializable]
	public sealed class AliasNamespaceReference : TypeOrNamespaceReference, ISupportsInterning
	{
		readonly string identifier;
		
		public AliasNamespaceReference(string identifier)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			this.identifier = identifier;
		}
		
		public string Identifier {
			get { return identifier; }
		}
		
		public override ResolveResult Resolve(VSharpResolver resolver)
		{
			return resolver.ResolveAlias(identifier);
		}
		
		public override IType ResolveType(VSharpResolver resolver)
		{
			// alias cannot refer to types
			return SpecialTypeSpec.UnknownType;
		}
		
		public override string ToString()
		{
			return identifier + "::";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return identifier.GetHashCode();
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			AliasNamespaceReference anr = other as AliasNamespaceReference;
			return anr != null && this.identifier == anr.identifier;
		}
	}
}
