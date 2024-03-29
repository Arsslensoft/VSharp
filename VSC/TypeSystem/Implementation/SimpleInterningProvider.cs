using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

using VSC.Base;

namespace VSC.TypeSystem.Implementation
{
	/// <summary>
	/// Simple interning provider.
	/// </summary>
	public sealed class SimpleInterningProvider : InterningProvider
	{
		sealed class InterningComparer : IEqualityComparer<ISupportsInterning>
		{
			public bool Equals(ISupportsInterning x, ISupportsInterning y)
			{
				return x.EqualsForInterning(y);
			}
			
			public int GetHashCode(ISupportsInterning obj)
			{
				return obj.GetHashCodeForInterning();
			}
		}
		
		sealed class ListComparer : IEqualityComparer<IEnumerable>
		{
			public bool Equals(IEnumerable a, IEnumerable b)
			{
				if (a.GetType() != b.GetType())
					return false;
				IEnumerator e1 = a.GetEnumerator();
				IEnumerator e2 = b.GetEnumerator();
				while (e1.MoveNext()) {
					// e1 has more elements than e2; or elements are different
					if (!e2.MoveNext() || e1.Current != e2.Current)
						return false;
				}
				if (e2.MoveNext()) // e2 has more elements than e1
					return false;
				// No need to dispose e1/e2: non-generic IEnumerator doesn't implement IDisposable,
				// and the underlying enumerator will likely be a List<T>.Enumerator which has an empty Dispose() method.
				return true;
			}
			
			public int GetHashCode(IEnumerable obj)
			{
				int hashCode = obj.GetType().GetHashCode();
				unchecked {
					foreach (object o in obj) {
						hashCode *= 27;
						hashCode += RuntimeHelpers.GetHashCode(o);
					}
				}
				return hashCode;
			}
		}
		
		Dictionary<object, object> byValueDict = new Dictionary<object, object>();
		Dictionary<ISupportsInterning, ISupportsInterning> supportsInternDict = new Dictionary<ISupportsInterning, ISupportsInterning>(new InterningComparer());
		Dictionary<IEnumerable, IEnumerable> listDict = new Dictionary<IEnumerable, IEnumerable>(new ListComparer());
		
		public override ISupportsInterning Intern(ISupportsInterning obj)
		{
			if (obj == null)
				return null;
			
			// ensure objects are frozen when we put them into the dictionary
			// note that Freeze may change the hash code of the object
			FreezableHelper.Freeze(obj);

			ISupportsInterning output;
			if (supportsInternDict.TryGetValue(obj, out output)) {
				return output;
			} else {
				supportsInternDict.Add(obj, obj);
				return obj;
			}
		}
		
		public override string Intern(string text)
		{
			if (text == null)
				return null;
			
			object output;
			if (byValueDict.TryGetValue(text, out output))
				return (string)output;
			else
				return text;
		}
		
		public override object InternValue(object obj)
		{
			if (obj == null)
				return null;
			
			object output;
			if (byValueDict.TryGetValue(obj, out output))
				return output;
			else
				return obj;
		}
		
		public override IList<T> InternList<T>(IList<T> list)
		{
			if (list == null)
				return null;
			if (list.Count == 0)
				return EmptyList<T>.Instance;
			if (!list.IsReadOnly)
				list = new ReadOnlyCollection<T>(list);
			IEnumerable output;
			if (listDict.TryGetValue(list, out output))
				list = (IList<T>)output;
			else
				listDict.Add(list, list);
			return list;
		}
	}
}
