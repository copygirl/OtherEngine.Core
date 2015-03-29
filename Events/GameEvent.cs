using System;
using System.Collections.Generic;
using OtherEngine.Core.Exceptions;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Events
{
	public abstract class GameEvent<TEvent> where TEvent : IEvent
	{
		public string Name { get; private set; }
		public GameSystem System { get; private set; }

		protected GameEvent(string name, GameSystem system)
		{
			Name = name;
			System = system;
		}

		public void Fire(TEvent ev)
		{
			System.Game.Events.GetSystems<IEventListener<TEvent>>().CheckedForeach(
				system => ((IEventListener<TEvent>)system).Receive(ev));
		}

		public override string ToString()
		{
			return string.Format(((System != null) ? "[Event: {1}.{0}]" : "[Event: {0}]"), Name, System);
		}
	}
}

