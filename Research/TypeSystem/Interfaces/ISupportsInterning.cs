using System;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Interface for TypeSystem objects that support interning.
	/// See <see cref="InterningProvider"/> for more information.
	/// </summary>
	public interface ISupportsInterning
	{
		/// <summary>
		/// Gets a hash code for interning.
		/// </summary>
		int GetHashCodeForInterning();
		
		/// <summary>
		/// Equality test for interning.
		/// </summary>
		bool EqualsForInterning(ISupportsInterning other);
	}
}
