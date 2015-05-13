using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Data
{
	/// <summary>
	/// Object which holds a number of components that can be looked up using their type.
	/// </summary>
	public class GameData : IEnumerable<GameComponent>
	{
		readonly Dictionary<Type, GameComponent> _components = new Dictionary<Type, GameComponent>();

		public int Count { get { return _components.Count; } }

		#region Looking up components

		/// <summary>
		/// Returns a component of the given type if it's inside this GameData, null otherwise.
		/// </summary>
		public GameComponent Get(Type componentType)
		{
			if (componentType == null)
				throw new ArgumentNullException("componentType");
			if (!typeof(GameComponent).IsAssignableFrom(componentType))
				throw new ArgumentException(String.Format(
					"{0} is not a GameComponent", componentType), "componentType");
			if (componentType.IsInterface || componentType.IsAbstract)
				throw new ArgumentException(String.Format(
					"{0} is not a concrete type that can be instantiated", componentType), "componentType");
			
			GameComponent component;
			_components.TryGetValue(componentType, out component);
			return component;
		}

		/// <summary>
		/// Returns a component of the given type if it's inside this GameData, null otherwise.
		/// </summary>
		public T Get<T>() where T : GameComponent
		{
			return (T)Get(typeof(T));
		}

		/// <summary>
		/// Returns a component of the given type. If one is not already inside this GameData, it will be created and added.
		/// </summary>
		public T GetOrCreate<T>() where T : GameComponent, new()
		{
			T item = (T)Get(typeof(T));
			if (item == null)
				Add(item = new T());
			return item;
		}

		#endregion

		#region Adding / removing components

		/// <summary>
		/// Adds a component to this GameData object.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the GameData object already contains a component of the same type.</exception>
		public virtual void Add(GameComponent component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			
			try {
				_components.Add(component.GetType(), component);
			} catch (ArgumentException) {
				throw new ArgumentException(String.Format(
					"{0} is already in {1}", component, this), "component");
			}
		}

		/// <summary>
		/// Removes a component from this GameData object.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the GameData object doesn't contain this component.</exception>
		public virtual void Remove(GameComponent component)
		{
			if (component == null)
				throw new ArgumentNullException("component");

			if (!_components.Remove(component.GetType()))
				throw new ArgumentException(String.Format(
					"{0} isn't in {1}", component, this), "component");
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<GameComponent> GetEnumerator()
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

