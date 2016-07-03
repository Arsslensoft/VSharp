using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a variable (name/type pair).
	/// </summary>
	public interface IVariable : ISymbol
	{
		/// <summary>
		/// Gets the name of the variable.
		/// </summary>
		new string Name { get; }
		
		/// <summary>
		/// Gets the declaration region of the variable.
		/// </summary>
		DomRegion Region { get; }
		
		/// <summary>
		/// Gets the type of the variable.
		/// </summary>
		IType Type { get; }
		
		/// <summary>
		/// Gets whether this variable is a constant (V#-like const).
		/// </summary>
		bool IsConst { get; }
		
		/// <summary>
		/// If this field is a constant, retrieves the value.
		/// For parameters, this is the default value.
		/// </summary>
		object ConstantValue { get; }
	}
}
