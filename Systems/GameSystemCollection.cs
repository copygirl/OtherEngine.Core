using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Collections;
using OtherEngine.Core.Exceptions;
using OtherEngine.Core.Events;

namespace OtherEngine.Core.Systems
{
	/// <summary>
	/// Container class for the game's systems.
	/// Handles enabling and disabling of systems, as well as errored ones.
	/// </summary>
	public class GameSystemCollection : IEnumerable<GameSystem>
	{
		private readonly SystemCollection _systems;

		private readonly Game _game;

		public readonly GameEvent<SystemEnabledEvent> Enabled = new GameEvent<SystemEnabledEvent>("Game.Systems.Enabled", null);
		public readonly GameEvent<SystemDisabledEvent> Disabled = new GameEvent<SystemDisabledEvent>("Game.Systems.Disabled", null);
		public readonly GameEvent<SystemErroredEvent> Errored = new GameEvent<SystemErroredEvent>("Game.Systems.Errored", null);

		public GameSystemCollection(Game game)
		{
			_game = game;
			_systems = new SystemCollection(game);
		}

		public T Get<T>() where T : GameSystem
		{
			return _systems.Get<T>(TypedGetBehavior.Default);
		}

		public void Enable<T>() where T : GameSystem
		{
			var system = _systems.Get<T>(TypedGetBehavior.CreateAndAdd);
			if (system.State >= GameSystemState.Enabled) return;

			try {
				system.State = GameSystemState.Enabled;
				system.OnEnabled();
				Enabled.Fire(new SystemEnabledEvent(system));
			} catch (Exception ex) {
				var gameEx = new GameSystemException(system, String.Format(
					"Exception when enabling {0}: {1}", system, ex.Message));
				OnError(system, gameEx);
				throw gameEx;
			}
		}
		public void Disable<T>() where T : GameSystem
		{
			var system = Get<T>();
			if ((system == null) || (system.State <= GameSystemState.Disabled)) return;

			try {
				system.State = GameSystemState.Disabled;
				system.OnDisabled();
				Disabled.Fire(new SystemDisabledEvent(system));
			} catch (Exception ex) {
				var gameEx = new GameSystemException(system, String.Format(
					"Exception when disabling {0}: {1}", system, ex.Message));
				OnError(system, gameEx);
				throw gameEx;
			}
		}

		public void OnError(GameSystem system, Exception ex)
		{
			if (system == null)
				throw new ArgumentNullException("system");
			// TODO: Do something with exception.
			system.State = GameSystemState.Errored;
			Console.Error.WriteLine(ex);
			Errored.Fire(new SystemErroredEvent(system, ex));
		}

		#region IEnumerable implementation

		public IEnumerator<GameSystem> GetEnumerator()
		{
			return _systems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private class SystemCollection : TypedDefaultCollection<GameSystem>
		{
			private readonly Game _game;

			public SystemCollection(Game game)
			{
				_game = game;
			}

			protected override GameSystem NewValue<TSystem>()
			{
				try {
					var system = Activator.CreateInstance<TSystem>();
					system.Game = _game;
					return system;
				} catch (MissingMethodException ex) {
					throw new GameSystemException(typeof(TSystem),
						String.Format("{0} does not have a parameterless constructor", typeof(TSystem)), ex);
				} catch (TargetInvocationException ex) {
					throw new GameSystemException(typeof(TSystem),
						String.Format("Exception in constructor of {0}: {1}", typeof(TSystem), ex.Message), ex);
				} catch (Exception ex) {
					throw new GameSystemException(typeof(TSystem),
						String.Format("Exception when constructing {0}: {1}", typeof(TSystem), ex.Message), ex);
				}
			}
		}
	}

	public class SystemEvent : IEvent
	{
		public GameSystem System { get; private set; }

		public SystemEvent(GameSystem system)
		{
			System = system;
		}
	}
	public class SystemEnabledEvent : SystemEvent
	{
		public SystemEnabledEvent(GameSystem system) : base(system) {  }
	}
	public class SystemDisabledEvent : SystemEvent
	{
		public SystemDisabledEvent(GameSystem system) : base(system) {  }
	}
	public class SystemErroredEvent : SystemEvent
	{
		public Exception Exception { get; private set; }

		public SystemErroredEvent(GameSystem system, Exception exception) : base(system)
		{
			Exception = exception;
		}
	}
}

