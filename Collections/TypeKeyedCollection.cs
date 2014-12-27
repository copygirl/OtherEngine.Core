using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace OtherEngine.Core.Collections
{
	public class TypeKeyedCollection<T> : EventedCollection<T>
	{
		private readonly IDictionary<string, T> dictionary = new Dictionary<string, T>();


		public T Get(string typeKey)
		{
			return dictionary[typeKey];
		}

		public TType Get<TType>() where TType : T
		{
			return (TType)Get(GetTypeString(typeof(TType)));
		}

		public void Remove(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			var typeKey = item.GetType().ToString();
			if (Removed != null)
				Removed(item);
		}

		#region implemented abstract members of EventedCollection

		public override int Count { get { return dictionary.Count; } }

		protected override bool AddInternal(T item)
		{
			var typeKey = GetTypeString(item);
			if (dictionary.ContainsKey(typeKey))
				throw new ArgumentException(String.Format("Duplicate type {0} in this {1}", typeKey, this), "item");
			dictionary.Add(typeKey, item);
			return true;
		}

		protected override bool ContainsInternal(T item)
		{
			var typeKey = GetTypeString(item);
			return dictionary.ContainsKey(typeKey);
		}

		protected override bool RemoveInternal(T item)
		{
			var typeKey = GetTypeString(item);
			if (!dictionary.Remove(typeKey))
				throw new ArgumentException(String.Format("Type {0} isn't in this {1}", typeKey, this), "item");
			return true;
		}

		protected override void ClearInternal()
		{
			dictionary.Clear();
		}

		public override void CopyTo(T[] array, int arrayIndex)
		{
			dictionary.Values.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return dictionary.Values.GetEnumerator();
		}

		#endregion

		private static string GetTypeString(Type type)
		{
			type.ToString();
		}

		private static string GetTypeString(object item)
		{
			return GetTypeString(item.GetType());
		}
	}
}

