using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace VSC.TypeSystem
{
    /// <summary>
	/// Represents a method or property.
	/// </summary>
	public interface IParameterizedMember : IMember
	{
		IList<IParameter> Parameters { get; }
	}
}
