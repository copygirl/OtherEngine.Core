using OtherEngine.Core.Data;

namespace OtherEngine.Core
{
	public class Game
	{
		public CoreSystemHandler Systems { get; private set; }
		public CoreEventHandler Events { get; private set; }

		public Game()
		{
			Systems = new CoreSystemHandler(this);
			Events = new CoreEventHandler(this);
		}

		internal void OnComponentAdded(GameEntity entity, GameComponent component)
		{
			//Events.Fire(typeof(GameComponentAddedEvent<>), component.GetType(), entity, component);
		}

		internal void OnComponentRemoved(GameEntity entity, GameComponent component)
		{
			//Events.Fire(typeof(GameComponentRemovedEvent<>), component.GetType(), entity, component);
		}
	}
}

