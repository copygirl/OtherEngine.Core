using System;
using OtherEngine.Core.Components;
using OtherEngine.Core.Events;

namespace OtherEngine.Core.Managers
{
	/// <summary>
	/// Handles firing component related events and component containers.
	/// </summary>
	public class ComponentManager : ContainerManager<Component>
	{
		internal ComponentManager(Game game) : base(game) {  }


		/// <summary>
		/// Called when a component is added to an entity.
		/// Fires the appropriate events.
		/// </summary>
		internal void OnAdded(Entity entity, Component component)
		{
			Game.Events.Fire(new ComponentAddedEvent(entity, component));

			// TODO: Speed this up using expressions.
			var type = typeof(ComponentAddedEvent<>).MakeGenericType(component.GetType());
			Game.Events.Fire((Event)Activator.CreateInstance(type, entity, component));
		}

		/// <summary>
		/// Called when a component is removed from an entity.
		/// Fires the appropriate events.
		/// </summary>
		internal void OnRemoved(Entity entity, Component component)
		{
			Game.Events.Fire(new ComponentRemovedEvent(entity, component));

			var type = typeof(ComponentRemovedEvent<>).MakeGenericType(component.GetType());
			Game.Events.Fire((Event)Activator.CreateInstance(type, entity, component));
		}


		protected override Entity CreateContainer(Type type)
		{
			var container = base.CreateContainer(type);
			container.Add(new NameComponent{ Value = Component.GetName(type) });
			return container;
		}
	}
}

