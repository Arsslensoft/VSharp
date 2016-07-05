using System;

namespace VSC.TypeSystem.Implementation
{
    [Serializable]
	public abstract class FreezableSpec : IFreezable
	{
		bool isFrozen;
		
		/// <summary>
		/// Gets if this instance is frozen. Frozen instances are immutable and thus thread-safe.
		/// </summary>
		public bool IsFrozen {
			get { return isFrozen; }
		}
		
		/// <summary>
		/// Freezes this instance.
		/// </summary>
		public void Freeze()
		{
			if (!isFrozen) {
				FreezeInternal();
				isFrozen = true;
			}
		}
		
		protected virtual void FreezeInternal()
		{
		}
	}
}
