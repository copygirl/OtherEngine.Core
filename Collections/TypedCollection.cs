using System;
using System.Collections.Generic;
using System.Linq;

namespace OtherEngine.Core.Collections
{
	/// <summary>
	/// Stores objects which can be looked up by their type.
	/// There can only be one instance of a specific type in the collection. 
	/// </summary>
	public class TypedCollection<T> : ICollection<T> where T : class
	{
		private readonly TypedDictionary<T, T> _dictionary = new TypedDictionary<T, T>();

		/// <summary>
		/// Returns the instance of type TType stored in this collection, or null if none.
		/// </summary>
		public TType Get<TType>() where TType : T
		{
			T item;
			return (_dictionary.TryGetValue<TType>(out item) ? (TType)item : null);
		}

		protected virtual void OnAdded(T value) {  }

		protected virtual void OnRemoved(T value) {  }

		#region ICollection implementation

		public bool IsReadOnly { get { return false; } }

		public int Count { get { return _dictionary.Count; } }

		public void Add(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			_dictionary.Add(item.GetType(), item);
			OnAdded(item);
		}

		public bool Contains(T item)
		{
			T i;
			return ((item != null) && _dictionary.TryGetValue(item.GetType(), out i) && (item == i));
		}

		public bool Remove(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			var removed = _dictionary.Remove(item.GetType());
			if (removed) OnRemoved(item);
			return removed;
		}

		public void Clear()
		{
			var items = _dictionary.Values.ToList();
			_dictionary.Clear();
			foreach (var item in items)
				OnRemoved(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_dictionary.Values.CopyTo(array, arrayIndex);
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<T> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

