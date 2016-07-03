using System;
using System.Collections.Generic;

namespace VSC.Base
{
	/// <summary>
	/// Contains extension methods for use within NRefactory.
	/// </summary>
	static class ExtensionMethods
	{
		public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> input)
		{
			foreach (T item in input)
				target.Add(item);
		}
		
		public static Predicate<T> And<T>(this Predicate<T> filter1, Predicate<T> filter2)
		{
			if (filter1 == null)
				return filter2;
			if (filter2 == null)
				return filter1;
			return m => filter1(m) && filter2(m);
		}
	}
}
