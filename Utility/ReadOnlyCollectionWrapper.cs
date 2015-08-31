using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Utility
{
	/// <summary>
	/// Wrapper for ICollection instances to only
	/// provide readonly access to the collection.
	/// </summary>
	public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>, ICollection<T>
	{
		public static ReadOnlyCollectionWrapper<T> Empty { get; }
			= new ReadOnlyCollectionWrapper<T>(new T[0]);


		readonly ICollection<T> _collection;


		public ReadOnlyCollectionWrapper(ICollection<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			_collection = collection;
		}


		#region IReadOnlyCollection implementation

		public int Count { get { return _collection.Count; } }

		#endregion

		#region ICollection implementation

		bool ICollection<T>.IsReadOnly { get { return true; } }

		void ICollection<T>.Add(T item) { throw new InvalidOperationException("Collection is readonly"); }

		bool ICollection<T>.Remove(T item) { throw new InvalidOperationException("Collection is readonly"); }

		void ICollection<T>.Clear() { throw new InvalidOperationException("Collection is readonly"); }

		public bool Contains(T item) { return _collection.Contains(item); }

		void ICollection<T>.CopyTo(T[] array, int arrayIndex) { _collection.CopyTo(array, arrayIndex); }

		#endregion

		#region IEnumerable implementation

		public IEnumerator<T> GetEnumerator()
		{
			return _collection.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

