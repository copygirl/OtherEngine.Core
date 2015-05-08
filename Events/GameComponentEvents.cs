using OtherEngine.Core.Data;

namespace OtherEngine.Core.Events
{
	public abstract class GameComponentEvent<TComponent> : IGameEvent where TComponent : GameComponent
	{
		public GameEntity Entity { get; private set; }
		public TComponent Component { get; private set; }

		protected GameComponentEvent(GameEntity entity, TComponent component)
		{
			Entity = entity;
			Component = component;
		}
	}

	/// <summary>
	/// Fired when a <see cref="GameComponent"/> is added to a <see cref="GameEntity"/>.
	/// </summary>
	public class GameComponentAddedEvent<TComponent> : GameComponentEvent<TComponent> where TComponent : GameComponent
	{
		internal GameComponentAddedEvent(GameEntity entity, TComponent component)
			: base(entity, component) {  }
	}

	/// <summary>
	/// Fired when a <see cref="GameComponent"/> is removed from a <see cref="GameEntity"/>.
	/// </summary>
	public class GameComponentRemovedEvent<TComponent> : GameComponentEvent<TComponent> where TComponent : GameComponent
	{
		internal GameComponentRemovedEvent(GameEntity entity, TComponent component)
			: base(entity, component) {  }
	}
}

