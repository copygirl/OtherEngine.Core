using System;
using System.Collections.Generic;
using System.Text;
using OtherEngine.Core.Components;

namespace OtherEngine.Core
{
	/// <summary>
	/// Basic engine object, capable of holding any number of components.
	/// </summary>
	public class Entity : ICollection<Component>
	{
		readonly Dictionary<Type, Component> _components =
			new Dictionary<Type, Component>();


		/// <summary>
		/// Gets the main game entity.
		/// </summary>
		public Game Game { get; private set; }


		public Entity(Game game)
		{
			if (game == null)
				throw new ArgumentNullException("game");

			Game = game;
		}


		#region Getting components

		public TComponent Get<TComponent>() where TComponent : Component
		{
			Component component;
			return (_components.TryGetValue(typeof(TComponent), out component)
				? (TComponent)component : null);
		}

		public TComponent GetOrCreate<TComponent>() where TComponent : Component, new()
		{
			var component = Get<TComponent>();
			if (component == null)
				Add(component = new TComponent());
			return component;
		}

		public TComponent GetOrThrow<TComponent>() where TComponent : Component
		{
			var component = Get<TComponent>();
			if (component == null)
				throw new ArgumentException(string.Format(
					"{0} is not in {1}", Component.ToString(typeof(TComponent)), this), "TComponent");
			return component;
		}

		public bool Has<TComponent>() where TComponent : Component
		{
			return (Get<TComponent>() != null);
		}

		#endregion

		#region Adding / removing components

		public void Add(Component component)
		{
			if (component == null)
				throw new ArgumentNullException("component");

			try { _components.Add(component.GetType(), component); }
			catch (ArgumentException ex) {
				throw new ArgumentException(string.Format(
					"{0} already has a {1}", this, component), "component", ex);
			}

			Game.Components.OnAdded(this, component);
		}

		bool ICollection<Component>.Remove(Component component)
		{
			var success = _components.Remove(component.GetType());
			if (success) Game.Components.OnRemoved(this, component);
			return success;
		}
		public void Remove(Component component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			if (!((ICollection<Component>)this).Remove(component))
				throw new ArgumentException(string.Format(
					"{0} doesn't have a {1}", this, component), "component");
		}

		#endregion


		#region ICollection implementation

		public int Count { get { return _components.Count; } }

		bool ICollection<Component>.IsReadOnly { get { return false; } }

		void ICollection<Component>.Clear()
		{
			foreach (var component in this)
				Remove(component);
		}

		bool ICollection<Component>.Contains(Component component)
		{
			Component test;
			return ((component != null) &&
				_components.TryGetValue(component.GetType(), out test) &&
				object.ReferenceEquals(component, test));
		}

		void ICollection<Component>.CopyTo(Component[] array, int index)
		{
			_components.Values.CopyTo(array, index);
		}

		public IEnumerator<Component> GetEnumerator()
		{
			return _components.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region ToString

		public override string ToString()
		{
			var sb = new StringBuilder("[");
			sb.Append(Get<TypeComponent>()?.Value ?? "Entity");

			var customName = Get<NameComponent>()?.Value;
			if (customName != null) sb.Append(" \"").Append(customName).Append('"');

			var guid = Get<GuidComponent>()?.Value;
			if (guid != null) sb.Append(" {").Append(guid).Append('}');

			if (Has<HierarchyLinkComponent>())
				sb.Append(": ").Append(Get<TargetComponent>()?.Value);
			
			return sb.Append(']').ToString();
		}

		#endregion
	}
}

