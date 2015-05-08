using System;
using OtherEngine.Core.Data;
using OtherEngine.Core.Events;
using System.Reflection;

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
			// TODO: If this is not fast enough, create an expression for every type of event.
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var type = typeof(GameComponentAddedEvent<>).MakeGenericType(component.GetType());
			Events.Fire((IGameEvent)Activator.CreateInstance(type, flags, null, new object[]{ entity, component }, null));
			Events.Fire(new GameComponentAddedEvent<GameComponent>(entity, component));
		}

		internal void OnComponentRemoved(GameEntity entity, GameComponent component)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var type = typeof(GameComponentRemovedEvent<>).MakeGenericType(component.GetType());
			Events.Fire((IGameEvent)Activator.CreateInstance(type, flags, null, new object[]{ entity, component }, null));
			Events.Fire(new GameComponentRemovedEvent<GameComponent>(entity, component));
		}
	}
}

