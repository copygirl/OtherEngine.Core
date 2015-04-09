namespace OtherEngine.Core.Events
{
	/// <summary>
	/// Base event for any events regarding IGameComponents.
	/// </summary>
	public abstract class GameComponentEvent<TComponent>
		: IGameEvent where TComponent : IGameComponent
	{
		public GameEntity Entity { get; private set; }
		public TComponent Component { get; private set; }

		public GameComponentEvent(GameEntity entity, TComponent component)
		{
			Entity = entity;
			Component = component;
		}
	}

	/// <summary>
	/// Event which is fired when an IGameComponent is added to any GameEntity.
	/// </summary>
	public class GameComponentAddedEvent<TComponent>
		: GameComponentEvent<TComponent> where TComponent : IGameComponent
	{
		public GameComponentAddedEvent(GameEntity entity, TComponent component)
			: base(entity, component) {  }
	}

	/// <summary>
	/// Event which is fired when an IGameComponent is removed from any GameEntity.
	/// </summary>
	public class GameComponentRemovedEvent<TComponent>
		: GameComponentEvent<TComponent> where TComponent : IGameComponent
	{
		public GameComponentRemovedEvent(GameEntity entity, TComponent component)
			: base(entity, component) {  }
	}
}

