using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace VSC.Base
{
	/// <summary>
	/// Allows caching values for a specific compilation.
	/// A CacheManager consists of a for shared instances (shared among all threads working with that resolve context).
	/// </summary>
	/// <remarks>This class is thread-safe</remarks>
	public sealed class CacheManager
	{
		readonly ConcurrentDictionary<object, object> sharedDict = new ConcurrentDictionary<object, object>(ReferenceComparer.Instance);
		// There used to be a thread-local dictionary here, but I removed it as it was causing memory
		// leaks in some use cases.
		
		public object GetShared(object key)
		{
			object value;
			sharedDict.TryGetValue(key, out value);
			return value;
		}
		
		public object GetOrAddShared(object key, Func<object, object> valueFactory)
		{
			return sharedDict.GetOrAdd(key, valueFactory);
		}
		
		public object GetOrAddShared(object key, object value)
		{
			return sharedDict.GetOrAdd(key, value);
		}
		
		public void SetShared(object key, object value)
		{
			sharedDict[key] = value;
		}
	}
}
