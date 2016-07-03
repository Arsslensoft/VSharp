using System;
using System.Collections.Generic;

namespace VSC.Base
{
	public sealed class ProjectedList<TInput, TOutput> : IList<TOutput> where TOutput : class
	{
		readonly IList<TInput> input;
		readonly Func<TInput, TOutput> projection;
		readonly TOutput[] items;
		
		public ProjectedList(IList<TInput> input, Func<TInput, TOutput> projection)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (projection == null)
				throw new ArgumentNullException("projection");
			this.input = input;
			this.projection = projection;
			this.items = new TOutput[input.Count];
		}
		
		public TOutput this[int index] {
			get {
				TOutput output = LazyInit.VolatileRead(ref items[index]);
				if (output != null) {
					return output;
				}
				return LazyInit.GetOrSet(ref items[index], projection(input[index]));
			}
		}
		
		TOutput IList<TOutput>.this[int index] {
			get { return this[index]; }
			set {
				throw new NotSupportedException();
			}
		}
		
		public int Count {
			get { return items.Length; }
		}
		
		bool ICollection<TOutput>.IsReadOnly {
			get { return true; }
		}
		
		int IList<TOutput>.IndexOf(TOutput item)
		{
			var comparer = EqualityComparer<TOutput>.Default;
			for (int i = 0; i < this.Count; i++) {
				if (comparer.Equals(this[i], item))
					return i;
			}
			return -1;
		}
		
		void IList<TOutput>.Insert(int index, TOutput item)
		{
			throw new NotSupportedException();
		}
		
		void IList<TOutput>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<TOutput>.Add(TOutput item)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<TOutput>.Clear()
		{
			throw new NotSupportedException();
		}
		
		bool ICollection<TOutput>.Contains(TOutput item)
		{
			var comparer = EqualityComparer<TOutput>.Default;
			for (int i = 0; i < this.Count; i++) {
				if (comparer.Equals(this[i], item))
					return true;
			}
			return false;
		}
		
		void ICollection<TOutput>.CopyTo(TOutput[] array, int arrayIndex)
		{
			for (int i = 0; i < items.Length; i++) {
				array[arrayIndex + i] = this[i];
			}
		}
		
		bool ICollection<TOutput>.Remove(TOutput item)
		{
			throw new NotSupportedException();
		}
		
		public IEnumerator<TOutput> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++) {
				yield return this[i];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
	
	public sealed class ProjectedList<TContext, TInput, TOutput> : IList<TOutput> where TOutput : class
	{
		readonly IList<TInput> input;
		readonly TContext context;
		readonly Func<TContext, TInput, TOutput> projection;
		readonly TOutput[] items;
		
		public ProjectedList(TContext context, IList<TInput> input, Func<TContext, TInput, TOutput> projection)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			if (projection == null)
				throw new ArgumentNullException("projection");
			this.input = input;
			this.context = context;
			this.projection = projection;
			this.items = new TOutput[input.Count];
		}
		
		public TOutput this[int index] {
			get {
				TOutput output = LazyInit.VolatileRead(ref items[index]);
				if (output != null) {
					return output;
				}
				return LazyInit.GetOrSet(ref items[index], projection(context, input[index]));
			}
		}
		
		TOutput IList<TOutput>.this[int index] {
			get { return this[index]; }
			set {
				throw new NotSupportedException();
			}
		}
		
		public int Count {
			get { return items.Length; }
		}
		
		bool ICollection<TOutput>.IsReadOnly {
			get { return true; }
		}
		
		int IList<TOutput>.IndexOf(TOutput item)
		{
			var comparer = EqualityComparer<TOutput>.Default;
			for (int i = 0; i < this.Count; i++) {
				if (comparer.Equals(this[i], item))
					return i;
			}
			return -1;
		}
		
		void IList<TOutput>.Insert(int index, TOutput item)
		{
			throw new NotSupportedException();
		}
		
		void IList<TOutput>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<TOutput>.Add(TOutput item)
		{
			throw new NotSupportedException();
		}
		
		void ICollection<TOutput>.Clear()
		{
			throw new NotSupportedException();
		}
		
		bool ICollection<TOutput>.Contains(TOutput item)
		{
			var comparer = EqualityComparer<TOutput>.Default;
			for (int i = 0; i < this.Count; i++) {
				if (comparer.Equals(this[i], item))
					return true;
			}
			return false;
		}
		
		void ICollection<TOutput>.CopyTo(TOutput[] array, int arrayIndex)
		{
			for (int i = 0; i < items.Length; i++) {
				array[arrayIndex + i] = this[i];
			}
		}
		
		bool ICollection<TOutput>.Remove(TOutput item)
		{
			throw new NotSupportedException();
		}
		
		public IEnumerator<TOutput> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++) {
				yield return this[i];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
