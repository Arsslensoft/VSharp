using System;
using System.Collections;
using System.Collections.Generic;

namespace VSC.Base.GoldParser.Grammar {
	/// <summary>
	/// A set class for grammar objects.
	/// </summary>
	/// <remarks>
	/// This class is being used because there is no <c>HashSet&lt;&gt;</c> class in the version 2 of the framework.
	/// </remarks>
	public class GrammarObjectSet<T>: IEnumerable<T> where T: GrammarObject<T> {
		private readonly Dictionary<T, bool> entries = new Dictionary<T, bool>();

		/// <summary>
		/// Includes or excludes the specified object from the set.
		/// </summary>
		public bool this[T obj] {
			get {
				bool result;
				return (obj != null) && entries.TryGetValue(obj, out result) && result;
			}
			set {
				if (value || entries.ContainsKey(obj)) {
					entries[obj] = value;
				}
			}
		}

		public bool Contains(T obj) {
			return this[obj];
		}

		/// <summary>
		/// Sets the specified object.
		/// </summary>
		/// <param name="obj">The object to be included.</param>
		/// <returns><c>true</c> if the object was not yet set.</returns>
		public bool Set(T obj) {
			bool isSet;
			if (entries.TryGetValue(obj, out isSet)) {
				if (!isSet) {
					entries[obj] = true;
				}
				return !isSet;
			}
			entries.Add(obj, true);
			return true;
		}

		public IEnumerator<T> GetEnumerator() {
			foreach (KeyValuePair<T, bool> entry in entries) {
				if (entry.Value) {
					yield return entry.Key;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
