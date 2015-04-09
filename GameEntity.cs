using System;
using System.Collections.Generic;
using OtherEngine.Core.Utility;
using OtherEngine.Core.Components;

namespace OtherEngine.Core
{
	/// <summary>
	/// Empty game object which holds a number of <see cref="IGameComponent"/>s.
	/// </summary>
	public sealed class GameEntity : IEnumerable<IGameComponent>
	{
		private IComponentNotifier _notifier;
		private Dictionary<string, IGameComponent> _components =
			new Dictionary<string, IGameComponent>();

		public GameEntity(IComponentNotifier notifier)
		{
			_notifier = notifier;
		}

		#region Components

		public void Add(IGameComponent component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			var key = TypeUtils.ToString(component);
			if (_components.ContainsKey(key))
				throw new ArgumentException(String.Format(
					"{0} is already in {1}", component, this), "component");
			_components.Add(key, component);

			_notifier.OnComponentAdded(this, component);
		}

		public void Remove(IGameComponent component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			var key = TypeUtils.ToString(component);
			if (!_components.Remove(key))
				throw new ArgumentException(String.Format(
					"{0} isn't in {1}", component, this), "component");

			_notifier.OnComponentRemoved(this, component);
		}

		public IGameComponent Get(string typeString)
		{
			if (typeString == null)
				throw new ArgumentNullException("typeString");
			IGameComponent component;
			return (_components.TryGetValue(typeString, out component) ? component : null);
		}
		public T Get<T>() where T : IGameComponent
		{
			return (T)Get(TypeUtils.ToString<T>());
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<IGameComponent> GetEnumerator()
		{
			return _components.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

