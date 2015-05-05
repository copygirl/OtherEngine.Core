using System;
using System.Collections.Generic;
using OtherEngine.Core.Components;
using OtherEngine.Core.Exceptions;
using OtherEngine.Core.Data;

namespace OtherEngine.Core
{
	/// <summary>
	/// Handles enabling, disabling and erroring of other GameSystems.
	/// </summary>
	public class CoreSystemHandler
	{
		readonly IDictionary<Type, GameEntity> _systems = new Dictionary<Type, GameEntity>();

		readonly Game _game;

		internal CoreSystemHandler(Game game)
		{
			_game = game;
		}

		/// <summary>
		/// Returns a GameSystem for the specified GameSystem type.
		/// </summary>
		public TSystem Get<TSystem>() where TSystem : GameSystem, new()
		{
			var container = GetContainer<TSystem>().Get<GameSystemContainerComponent>();
			return (TSystem)(container.System ?? (container.System = new TSystem{
				Game = _game, State = GameSystemState.Disabled }));
		}

		#region GameSystem container

		/// <summary>
		/// Returns a system container GameEntity for the specified GameSystem type
		/// which can be used to store information related to that GameSystem.
		/// </summary>
		public GameEntity GetContainer<TSystem>() where TSystem : GameSystem, new()
		{
			return GetContainer(typeof(TSystem));
		}

		/// <summary>
		/// Returns a system container GameEntity for the specified GameSystem
		/// which can be used to store information related to that GameSystem.
		/// </summary>
		public GameEntity GetContainer(GameSystem system) 
		{
			if (system == null)
				throw new ArgumentNullException("system");
			return GetContainer(system.GetType());
		}

		/// <summary>
		/// Returns a system container GameEntity for the specified GameSystem type
		/// which can be used to store information related to that GameSystem.
		/// </summary>
		public GameEntity GetContainer(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsSubclassOf(typeof(GameSystem)))
				throw new ArgumentException(string.Format("{0} is not a GameSystem", type), "type");

			GameEntity container;
			if (!_systems.TryGetValue(type, out container))
				_systems.Add(type, (container = new GameEntity(_game){ new GameSystemContainerComponent(type) }));
			return container;
		}

		#endregion

		#region GameSystem state changing

		internal void Enable(GameSystem system)
		{
			SetState(system, GameSystemState.Running, OnSystemEnabled);
		}
		internal void Disable(GameSystem system)
		{
			SetState(system, GameSystemState.Disabled, OnSystemDisabled);
		}
		private void SetState(GameSystem system, GameSystemState state, Action<GameSystem> action = null)
		{
			if (system.State == state)
				throw new GameSystemStateException(system, String.Format(
					"{0} is already {1}.", system, state.ToString().ToLowerInvariant()));

			var previousState = system.State;
			system.State = state;

			if (action != null) {
				try { action(system); }
				catch (GameSystemException) { throw; }
				catch (Exception ex) {
					throw OnException(system, new GameSystemException(system, string.Format(
						"Exception when setting state of {0} to {1}: {2}", system, state, ex), ex));
				}
			}

			FireSystemStateChanged(system, previousState);
		}

		private void OnSystemEnabled(GameSystem system)
		{
			_game.Events.OnSystemEnabled(system);
			system.OnEnabled();
		}
		private void OnSystemDisabled(GameSystem system)
		{
			_game.Events.OnSystemDisabled(system);
			system.OnDisabled();
		}

		private void FireSystemStateChanged(GameSystem system, GameSystemState previousState)
		{
			Type type;
			if (system.State.IsEnabled() && previousState.IsDisabled())
				type = typeof(GameSystemEnabledEvent<>);
			else if (system.State.IsDisabled() && previousState.IsEnabled())
				type = typeof(GameSystemDisabledEvent<>);
			else type = typeof(GameSystemStateChangedEvent<>);

			_game.Events.Fire(type, system.GetType(), system, previousState);
		}

		/// <summary>
		/// Called when a GameSystem caused an error, for example as the system is
		/// being enabled or disabled or an event fired at it throws an exception.
		/// </summary>
		internal Exception OnException(GameSystem system, Exception ex)
		{
			if (!system.State.IsErrored()) {
				var previousState = system.State;
				system.State = GameSystemState.Errored;
				FireSystemStateChanged(system, previousState);
			}
			// TODO: Do something with exception.
			Console.Error.WriteLine(ex);
			// FIXME: Currently throws Exception for easy debugging.
			throw ex;
		}

		#endregion
	}
}

