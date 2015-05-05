using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Data
{
	/// <summary>
	/// Object which holds a number of items that can be looked up using their type.
	/// </summary>
	public class GameData<TItem> : IEnumerable<TItem>
	{
		readonly Dictionary<Type, TItem> _items = new Dictionary<Type, TItem>();

		public int Count { get { return _items.Count; } }

		#region Looking up components

		/// <summary>
		/// Returns an instance of the given type if it's inside this GameData, null otherwise.
		/// </summary>
		public TItem Get(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(type);
			if (typeof(TItem).IsAssignableFrom(type))
				throw new ArgumentException(String.Format(
					"{0} is not an {1}", type, typeof(TItem)), "type");
			if (type.IsInterface || type.IsAbstract)
				throw new ArgumentException(String.Format(
					"{0} is not a concrete type that can be instantiated", type), "type");
			
			TItem item;
			_items.TryGetValue(type, out item);
			return item;
		}

		/// <summary>
		/// Returns an instance of the given type if it's inside this GameData, null otherwise.
		/// </summary>
		public T Get<T>() where T : TItem
		{
			return (T)Get(typeof(T));
		}

		/// <summary>
		/// Returns an instance of the given type. If one is not already inside this GameData, it will be created and added.
		/// </summary>
		public T GetOrCreate<T>() where T : TItem, new()
		{
			T item = (T)Get(typeof(T));
			if (item == null)
				Add(item = new T());
			return item;
		}

		#endregion

		#region Adding / removing components

		/// <summary>
		/// Adds an item to this GameData object.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the GameData object already contains a item of the same type.</exception>
		public virtual void Add(TItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			try {
				_items.Add(item.GetType(), item);
			} catch (ArgumentException ex) {
				throw new ArgumentException(String.Format(
					"{0} is already in {1}", item, this), "item");
			}
		}

		/// <summary>
		/// Removes an item from this GameData object.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the GameData object doesn't contain this item.</exception>
		public virtual void Remove(TItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (!_items.Remove(item.GetType()))
				throw new ArgumentException(String.Format(
					"{0} isn't in {1}", item, this), "item");
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<TItem> GetEnumerator()
		{
			return _items.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

