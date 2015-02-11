using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Collections
{
	public class TypedCollection<T> : ICollection<T>
	{
		private readonly TypedDictionary<T, T> _dictionary = new TypedDictionary<T, T>();

		public TType Get<TType>() where TType : T
		{
			return _dictionary.Get<TType, TType>();
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
			return ((item != null) ? _dictionary.ContainsKey(item.GetType()) : false);
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

			_dictionary.Clear();
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

