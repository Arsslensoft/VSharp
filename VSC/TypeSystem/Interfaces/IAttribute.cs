using System;
using System.Collections.Generic;
using VSC.Base;


namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents an unresolved attribute.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public interface IUnresolvedAttribute
	{
		/// <summary>
		/// Gets the code region of this attribute.
		/// </summary>
		DomRegion Region { get; }
		
		/// <summary>
		/// Resolves the attribute.
		/// </summary>
		IAttribute CreateResolvedAttribute(ITypeResolveContext context);
	}
	
	/// <summary>
	/// Represents an attribute.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public interface IAttribute
	{
		/// <summary>
		/// Gets the code region of this attribute.
		/// </summary>
		DomRegion Region { get; }
		
		/// <summary>
		/// Gets the type of the attribute.
		/// </summary>
		IType AttributeType { get; }
		
		/// <summary>
		/// Gets the constructor being used.
		/// This property may return null if no matching constructor was found.
		/// </summary>
		IMethod Constructor { get; }
		
		/// <summary>
		/// Gets the positional arguments.
		/// </summary>
		IList<ResolveResult> PositionalArguments { get; }
		
		/// <summary>
		/// Gets the named arguments passed to the attribute.
		/// </summary>
		IList<KeyValuePair<IMember, ResolveResult>> NamedArguments { get; }
	}
}
