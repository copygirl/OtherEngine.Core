using System;
using System.Collections.Generic;
using OtherEngine.Core.Exceptions;

namespace OtherEngine.Core.Events
{
	public abstract class GameEvent
	{
		protected readonly HashSet<GameSystem> _listeners = new HashSet<GameSystem>();

		public string Name { get; private set; }
		public GameSystem System { get; private set; }

		protected GameEvent(string name, GameSystem system)
		{
			Name = name;
			System = system;
		}

		protected void Register(GameSystem system)
		{
			if (system == null)
				throw new ArgumentNullException();
			if (system.State <= GameSystemState.Disabled)
				throw new GameSystemException(system, String.Format("{0} can't start listening to {1}", system, this));
			if (!_listeners.Add(system))
				throw new GameSystemException(system, String.Format("{0} is already listening to {1}", system, this));
			system._events.Add(this);
		}

		public void Unregister(GameSystem system)
		{
			if (system == null)
				throw new ArgumentNullException();
			if (!_listeners.Remove(system))
				throw new GameSystemException(system, String.Format("{0} is not listening to {1}", system, this));
			system._events.Remove(this);
		}

		public override string ToString()
		{
			return string.Format(((System != null) ? "[Event: {1}.{0}]" : "[Event: {0}]"), Name, System);
		}
	}

	public class GameEvent<TEvent> : GameEvent where TEvent : IEvent
	{
		public GameEvent(string name, GameSystem system) : base(name, system) {  }

		public void Register<T>(T system) where T : GameSystem, IEventListener<TEvent>
		{
			base.Register(system);
		}

		public void Fire(TEvent ev)
		{
			foreach (var system in _listeners) {
				try { ((IEventListener<TEvent>)system).Receive(ev); }
				catch (Exception ex) { system.Game.Systems.OnError(system, ex); }
			}
		}
	}
}

