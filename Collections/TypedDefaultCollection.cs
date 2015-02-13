using System.Collections.Generic;
using OtherEngine.Core.Collections;

namespace OtherEngine.Core.Collections
{
	/// <summary>
	/// Abstract collection which provides default values depending on its implementation.
	/// </summary>
	public abstract class TypedDefaultCollection<TType, TValue> : IEnumerable<TValue>
	{
		private readonly TypedDictionary<TType, TValue> _dictionary =
			new TypedDictionary<TType, TValue>();

		/// <summary>
		/// Gets a value from the collection. If the value doesn't already exist in the
		/// collection, the return value depends on the <see cref="TypedGetBehavior"/>.
		/// </summary>
		public TValue Get<T>(TypedGetBehavior behavior) where T : TType
		{
			TValue value = default(TValue);
			if (!_dictionary.TryGetValue<T>(out value) && (behavior > TypedGetBehavior.Default)) {
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
		/// <summary>Returns default(T), null for reference values.</summary>
		Default,
		/// <summary>Returns a new value, but doesn't add it.</summary>
		Create,
		/// <summary>Returns a new value and adds it to the collection.</summary>
		CreateAndAdd
	}
}

