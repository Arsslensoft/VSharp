using System;
using System.Collections.Generic;

namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// Represents a type resolve result that's resolved using an alias.
	/// </summary>
	public class AliasTypeResolveResult : TypeResolveResult
	{
		/// <summary>
		/// The alias used.
		/// </summary>
		public string Alias {
			get;
			private set;
		}
		
		public AliasTypeResolveResult(string alias, TypeResolveResult underlyingResult) : base (underlyingResult.Type)
		{
			this.Alias = alias;
		}
	}
}
