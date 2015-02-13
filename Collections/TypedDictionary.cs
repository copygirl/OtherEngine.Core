using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Collections
{
	/// <summary>
	/// Dictionary whose values are looked up by a type.
	/// Child class of Dictionary for sake of simplicity.
	/// </summary>
	public class TypedDictionary<TType, TValue> : Dictionary<Type, TValue>
	{
		public void Add<T>(TValue item) where T : TType
		{
			Add(typeof(T), item);
		}

		public TValue Get<T>() where T : TType
		{
			return this[typeof(T)];
		}

		public bool TryGetValue<T>(out TValue item) where T : TType
		{
			return TryGetValue(typeof(T), out item);
		}

		public bool Contains<T>() where T : TType
		{
			return ContainsKey(typeof(T));
		}

		public bool Remove<T>() where T : TType
		{
			return Remove(typeof(T));
		}
	}
}

