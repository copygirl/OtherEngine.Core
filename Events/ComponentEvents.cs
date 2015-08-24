
namespace OtherEngine.Core.Events
{
	/// <summary>
	/// Base class for other component events.
	/// </summary>
	public abstract class ComponentEvent : Event
	{
		public Entity Entity { get; private set; }
		public Component Component { get; private set; }

		protected ComponentEvent(Entity entity, Component component)
		{
			Entity = entity;
			Component = component;
		}
	}


	/// <summary>
	/// Fired when a component is added to an entity.
	/// </summary>
	public class ComponentAddedEvent : ComponentEvent
	{
		public ComponentAddedEvent(Entity entity, Component component)
			: base(entity, component) {  }
	}

	/// <summary>
	/// Fired when a specific component type is added to an entity.
	/// </summary>
	public class ComponentAddedEvent<TComponent> : ComponentAddedEvent
		where TComponent : Component
	{
		public new TComponent Component { get { return (TComponent)base.Component; } }

		public ComponentAddedEvent(Entity entity, TComponent component)
			: base(entity, component) {  }
	}


	/// <summary>
	/// Fired when a component is removed from an entity.
	/// </summary>
	public class ComponentRemovedEvent : ComponentEvent
	{
		public ComponentRemovedEvent(Entity entity, Component component)
			: base(entity, component) {  }
	}

	/// <summary>
	/// Fired when a specific component type is removed from an entity.
	/// </summary>
	public class ComponentRemovedEvent<TComponent> : ComponentRemovedEvent
		where TComponent : Component
	{
		public new TComponent Component { get { return (TComponent)base.Component; } }

		public ComponentRemovedEvent(Entity entity, TComponent component)
			: base(entity, component) {  }
	}
}

