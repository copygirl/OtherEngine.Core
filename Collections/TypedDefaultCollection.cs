using System;
using System.Collections.Generic;
using System.Linq;
using OtherEngine.Core.Utility;
using OtherEngine.Core.Collections;

namespace OtherEngine.Core.Collections
{
	public abstract class TypedDefaultCollection<TType, TValue> : IEnumerable<TValue>
	{
		private readonly TypedDictionary<TType, TValue> _dictionary =
			new TypedDictionary<TType, TValue>();

		public IEnumerable<TValue> Values { get { return _dictionary.Values; } }

		public TValue Get<T>(TypedGetBehavior behavior) where T : TType
		{
			TValue value = default(TValue);
			if (!_dictionary.TryGetValue<T, TValue>(out value) && (behavior > TypedGetBehavior.Default)) {
				value = NewValue<T>();
				if (behavior >= TypedGetBehavior.CreateAndAdd)
					_dictionary.Add<T>(value);
			}
			return value;
		}

		public bool Remove<T>() where T : TType
		{
			return _dictionary.Remove<T>();
		}

		protected abstract TValue NewValue<T>() where T : TType;

		#region IEnumerable implementation

		public IEnumerator<TValue> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

	public abstract class TypedDefaultCollection<TType> : TypedDefaultCollection<TType, TType>
	{
		public new T Get<T>(TypedGetBehavior behavior) where T : TType
		{
			return (T)base.Get<T>(behavior);
		}
	}

	public enum TypedGetBehavior
	{
		Default,
		Create,
		CreateAndAdd
	}
}

