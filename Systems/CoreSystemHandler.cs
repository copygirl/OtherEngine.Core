using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Collections;
using OtherEngine.Core.Events;
using OtherEngine.Core.Exceptions;

namespace OtherEngine.Core.Systems
{
	/// <summary>
	/// Container class for the game's systems.
	/// Handles enabling and disabling of systems, as well as errored ones.
	/// </summary>
	public class CoreSystemHandler : IEnumerable<GameSystem>
	{
		private readonly Game _game;
		private readonly SystemCollection _systems;

		internal CoreSystemHandler(Game game)
		{
			_game = game;
			_systems = new SystemCollection(this);
		}

		public T Get<T>() where T : GameSystem
		{
			return _systems.Get<T>(TypedGetBehavior.Default);
		}

		public void Enable<T>() where T : GameSystem
		{
			var system = _systems.Get<T>(TypedGetBehavior.CreateAndAdd);
			if (system.State >= GameSystemState.Running) return;

			try {

				system.State = GameSystemState.Running;
				system.OnEnabled();
				_game.Events.GetSystems<ISystemWatcher>().CheckedForeach(
					s => ((ISystemWatcher)s).OnSystemEnabled(system));
				_game.Events.OnSystemEnabled(system);

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
				_game.Events.OnSystemDisabled(system);
				_game.Events.GetSystems<ISystemWatcher>().CheckedForeach(
					s => ((ISystemWatcher)s).OnSystemDisabled(system));

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
			system.State = GameSystemState.Errored;
			_game.Events.OnSystemDisabled(system);
			_game.Events.GetSystems<ISystemWatcher>().CheckedForeach(
				s => ((ISystemWatcher)s).OnSystemErrored(system));

			// TODO: Do something with exception.
			Console.Error.WriteLine(ex);
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
			private readonly CoreSystemHandler _system;

			public SystemCollection(CoreSystemHandler system)
			{
				_system = system;
			}

			protected override GameSystem NewValue<TSystem>()
			{
				try {
					var system = Activator.CreateInstance<TSystem>();
					system.Game = _system._game;
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
}

