using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VSC.Base;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a simple V# name. (a single non-qualified identifier with an optional list of type arguments)
	/// </summary>
	[Serializable]
	public sealed class SimpleTypeOrNamespaceReference : TypeOrNamespaceReference, ISupportsInterning
	{
		readonly string identifier;
		readonly IList<ITypeReference> typeArguments;
		readonly NameLookupMode lookupMode;
		
		public SimpleTypeOrNamespaceReference(string identifier, IList<ITypeReference> typeArguments, NameLookupMode lookupMode = NameLookupMode.Type)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			this.identifier = identifier;
			this.typeArguments = typeArguments ?? EmptyList<ITypeReference>.Instance;
			this.lookupMode = lookupMode;
		}
		
		public string Identifier {
			get { return identifier; }
		}
		
		public IList<ITypeReference> TypeArguments {
			get { return typeArguments; }
		}
		
		public NameLookupMode LookupMode {
			get { return lookupMode; }
		}
		
		/// <summary>
		/// Adds a suffix to the identifier.
		/// Does not modify the existing type reference, but returns a new one.
		/// </summary>
		public SimpleTypeOrNamespaceReference AddSuffix(string suffix)
		{
			return new SimpleTypeOrNamespaceReference(identifier + suffix, typeArguments, lookupMode);
		}
		
		public override ResolveResult Resolve(VSharpResolver resolver)
		{
			var typeArgs = typeArguments.Resolve(resolver.CurrentTypeResolveContext);
			return resolver.LookupSimpleNameOrTypeName(identifier, typeArgs, lookupMode);
		}

        public override IType ResolveType(VSharpResolver resolver)
		{
			TypeResolveResult trr = Resolve(resolver) as TypeResolveResult;
			return trr != null ? trr.Type : new UnknownTypeSpec(null, identifier, typeArguments.Count);
		}
		
		public override string ToString()
		{
			if (typeArguments.Count == 0)
				return identifier;
			else
				return identifier + "<" + string.Join(",", typeArguments) + ">";
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000021 * identifier.GetHashCode();
				hashCode += 1000000033 * typeArguments.GetHashCode();
				hashCode += 1000000087 * (int)lookupMode;
			}
			return hashCode;
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			SimpleTypeOrNamespaceReference o = other as SimpleTypeOrNamespaceReference;
			return o != null && this.identifier == o.identifier
				&& this.typeArguments == o.typeArguments && this.lookupMode == o.lookupMode;
		}
	}
}
