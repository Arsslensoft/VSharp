using System;

namespace VSC.TypeSystem
{
	public interface IFreezable
	{
		/// <summary>
		/// Gets if this instance is frozen. Frozen instances are immutable and thus thread-safe.
		/// </summary>
		bool IsFrozen { get; }
		
		/// <summary>
		/// Freezes this instance.
		/// </summary>
		void Freeze();
	}
}
