using System;
using System.Diagnostics.Contracts;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Represents a field or constant.
	/// </summary>
	public interface IField : IMember, IVariable
	{
		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		new string Name { get; } // solve ambiguity between IMember.Name and IVariable.Name
		
		/// <summary>
		/// Gets the region where the field is declared.
		/// </summary>
		new DomRegion Region { get; } // solve ambiguity between IEntity.Region and IVariable.Region
		
		/// <summary>
		/// Gets whether this field is readonly.
		/// </summary>
		bool IsReadOnly { get; }
		
		
		new IMemberReference ToReference(); // solve ambiguity between IMember.ToReference() and IVariable.ToReference()
	}
}
