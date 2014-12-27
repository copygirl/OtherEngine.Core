using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Collections
{
	public abstract class EventedCollection<T> : ICollection<T>
	{
		public event Action<T> Added;
		public event Action<T> Removed;


		protected virtual void OnAdded(T item)
		{
			if (Added != null)
				Added(item);
		}

		protected virtual void OnRemoved(T item)
		{
			if (Removed != null)
				Removed(item);
		}


		protected abstract bool AddInternal(T item);

		protected abstract bool ContainsInternal(T item);

		protected abstract bool RemoveInternal(T item);

		protected abstract void ClearInternal();


		#region ICollection implementation

		public abstract int Count { get; }

		public bool IsReadOnly { get { return false; } }

		public void Add(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (AddInternal(item))
				OnAdded(item);
		}

		public bool Contains(T item)
		{
			return ((item != null) ? ContainsInternal(item) : false);
		}

		public bool Remove(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (RemoveInternal(item))
				OnRemoved(item);
			return true;
		}

		public void Clear()
		{
			T[] array = new T[Count];
			CopyTo(array, 0);
			ClearInternal();
			foreach (var item in array)
				OnRemoved(item);
		}

		public abstract void CopyTo(T[] array, int arrayIndex);

		#endregion

		#region IEnumerable implementation

		public abstract IEnumerator<T> GetEnumerator();

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

