using System;
using System.Collections.Generic;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a namespace resolve result that's resolved using an alias.
	/// </summary>
	public class AliasNamespaceResolveResult : NamespaceResolveResult
	{
		/// <summary>
		/// The alias used.
		/// </summary>
		public string Alias {
			get;
			private set;
		}
		
		public AliasNamespaceResolveResult(string alias, NamespaceResolveResult underlyingResult) : base (underlyingResult.Namespace)
		{
			this.Alias = alias;
		}
	}
}

