using System.Collections.Generic;

namespace OtherEngine.Core.Utility
{
	public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
	{
		readonly ICollection<T> _collection;

		public ReadOnlyCollectionWrapper(ICollection<T> collection)
		{
			_collection = collection;
		}

		#region IReadOnlyCollection implementation

		public int Count { get { return _collection.Count; } }

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

