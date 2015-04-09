namespace OtherEngine.Core.Events
{
	/// <summary>
	/// Base event for any events regarding GameSystems.
	/// </summary>
	public abstract class GameSystemEvent<TSystem>
		: IGameEvent where TSystem : GameSystem
	{
		public TSystem System { get; private set; }

		public GameSystemEvent(TSystem system)
		{
			System = system;
		}
	}

	/// <summary>
	/// Event which is fired when a GameSystem's state changes.
	/// See GameSystemState for possible states.
	/// </summary>
	public class GameSystemStateChangedEvent<TSystem>
		: GameSystemEvent<TSystem> where TSystem : GameSystem
	{
		public GameSystemState PreviousState { get; private set; }
		public GameSystemState CurrentState { get; private set; }

		public GameSystemStateChangedEvent(TSystem system, GameSystemState previousState)
			: base(system)
		{
			PreviousState = previousState;
			CurrentState = system.State;
		}
	}

	/// <summary>
	/// Event which is fired when a GameSystem is enabled.
	/// </summary>
	public class GameSystemEnabledEvent<TSystem>
		: GameSystemStateChangedEvent<TSystem> where TSystem : GameSystem
	{
		public GameSystemEnabledEvent(TSystem system, GameSystemState previousState)
			: base(system, previousState) {  }
	}

	/// <summary>
	/// Event which is fired when a GameSystem is disabled.
	/// Is also fired when the system errores (as long as it was previously enabled).
	/// </summary>
	public class GameSystemDisabledEvent<TSystem>
		: GameSystemStateChangedEvent<TSystem> where TSystem : GameSystem
	{
		public GameSystemDisabledEvent(TSystem system, GameSystemState previousState)
			: base(system, previousState) {  }
	}
}

