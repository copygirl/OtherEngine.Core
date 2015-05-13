using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Components;
using OtherEngine.Core.Data;
using OtherEngine.Core.Events;
using OtherEngine.Core.Exceptions;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core
{
	/// <summary>
	/// Handles enabling, disabling and erroring of all <see cref="GameSystem"/>s.
	/// When enumerated, returns all systems of this <see cref="Game"/> instance
	/// regardless of their state.
	/// </summary>
	public class CoreSystemHandler : IEnumerable<GameSystem>
	{
		readonly IDictionary<Type, GameData> _systemContainers = new Dictionary<Type, GameData>();
		readonly ICollection<GameSystem> _systems = new HashSet<GameSystem>();

		readonly Game _game;

		internal CoreSystemHandler(Game game)
		{
			_game = game;
		}

		/// <summary>
		/// Returns a <see cref="GameSystem"/> for the specified GameSystem type, creating it
		/// if necessary. Returns null if there was an exception instantiating it.
		/// </summary>
		public TSystem Get<TSystem>() where TSystem : GameSystem, new()
		{
			var container = GetContainer<TSystem>().Get<GameSystemContainerComponent>();
			if ((container.System == null) && !container.ConstructorThrewException) {
				try {
					container.System = new TSystem { Game = _game, State = GameSystemState.Disabled };
					_systems.Add(container.System);
				} catch {
					container.ConstructorThrewException = true;
					throw;
				}
			}
			return (TSystem)container.System;
		}

		#region GameSystem container

		/// <summary>
		/// Returns a system container <see cref="GameData"/> for the specified <see cref="GameSystem"/>
		/// type which can be used to store information related to that GameSystem.
		/// </summary>
		public GameData GetContainer<TSystem>() where TSystem : GameSystem, new()
		{
			return GetContainer(typeof(TSystem));
		}

		/// <summary>
		/// Returns a system container <see cref="GameData"/> for the specified <see cref="GameSystem"/>
		/// which can be used to store information related to that GameSystem.
		/// </summary>
		public GameData GetContainer(GameSystem system) 
		{
			if (system == null)
				throw new ArgumentNullException("system");
			return GetContainer(system.GetType());
		}

		/// <summary>
		/// Returns a system container <see cref="GameData"/> for the specified <see cref="GameSystem"/>
		/// which can be used to store information related to that GameSystem.
		/// </summary>
		public GameData GetContainer(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsSubclassOf(typeof(GameSystem)))
				throw new ArgumentException(string.Format("{0} is not a GameSystem", type), "type");

			GameData container;
			if (!_systemContainers.TryGetValue(type, out container))
				_systemContainers.Add(type, (container = new GameData { new GameSystemContainerComponent(type) }));
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
			action(system);
		}

		private void OnSystemEnabled(GameSystem system)
		{
			_game.Events.OnSystemEnabled(system);
			system.OnEnabled();

			var flags = BindingFlags.Instance.Public.NonPublic;
			var type = typeof(GameSystemEnabledEvent<>).MakeGenericType(system.GetType());
			_game.Events.Fire((IGameEvent)Activator.CreateInstance(type, flags, null, new object[]{ system }, null));
			_game.Events.Fire(new GameSystemEnabledEvent<GameSystem>(system));
		}
		private void OnSystemDisabled(GameSystem system)
		{
			_game.Events.OnSystemDisabled(system);
			system.OnDisabled();

			var flags = BindingFlags.Instance.Public.NonPublic;
			var type = typeof(GameSystemDisabledEvent<>).MakeGenericType(system.GetType());
			_game.Events.Fire((IGameEvent)Activator.CreateInstance(type, flags, null, new object[]{ system }, null));
			_game.Events.Fire(new GameSystemDisabledEvent<GameSystem>(system));
		}

		#endregion

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
	}
}

