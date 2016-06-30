using System;
using System.Threading;

namespace VSC.Base
{
	public static class LazyInit
	{
		public static T VolatileRead<T>(ref T location) where T : class
		{
			#if NET_4_5
			return Volatile.Read(ref location);
			#else
			T result = location;
			Thread.MemoryBarrier();
			return result;
			#endif
		}
		
		/// <summary>
		/// Atomically performs the following operation:
		/// - If target is null: stores newValue in target and returns newValue.
		/// - If target is not null: returns target.
		/// </summary>
		public static T GetOrSet<T>(ref T target, T newValue) where T : class
		{
			T oldValue = Interlocked.CompareExchange(ref target, newValue, null);
			return oldValue ?? newValue;
		}
	}
}
