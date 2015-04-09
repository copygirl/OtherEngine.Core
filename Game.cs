using System;
using OtherEngine.Core.Components;
using OtherEngine.Core.Events;

namespace OtherEngine.Core
{
	public class Game : IComponentNotifier
	{
		public CoreSystemHandler Systems { get; private set; }
		public CoreEventHandler Events { get; private set; }

		public Game()
		{
			Systems = new CoreSystemHandler(this);
			Events = new CoreEventHandler(this);
		}

		#region IComponentNotifier implementation

		public void OnComponentAdded(GameEntity entity, IGameComponent component)
		{
			Events.Fire(typeof(GameComponentAddedEvent<>), component.GetType(), entity, component);
		}

		public void OnComponentRemoved(GameEntity entity, IGameComponent component)
		{
			Events.Fire(typeof(GameComponentRemovedEvent<>), component.GetType(), entity, component);
		}

		#endregion
	}
}

