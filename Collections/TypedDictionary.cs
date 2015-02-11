using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Collections
{
	public class TypedDictionary<TType, TValue> : Dictionary<Type, TValue>
	{
		public void Add<T>(TValue item) where T : TType
		{
			Add(typeof(T), item);
		}

		public TValue Get<T>() where T : TType
		{
			TValue item;
			return (TryGetValue(typeof(T), out item) ? item : default(TValue));
		}
		public TOut Get<T, TOut>() where T : TType where TOut : TValue
		{
			return (TOut)Get<T>();
		}

		public bool TryGetValue<T>(out TValue item) where T : TType
		{
			return TryGetValue(typeof(T), out item);
		}
		public bool TryGetValue<T, TOut>(out TOut item) where T : TType where TOut : TValue
		{
			TValue i;
			bool ret = TryGetValue(typeof(T), out i);
			item = (TOut)i;
			return ret;
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

