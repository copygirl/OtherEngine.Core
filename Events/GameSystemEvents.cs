using System;
using OtherEngine.Core.Events;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Events
{
	public abstract class GameSystemEvent<TSystem> : IGameEvent where TSystem : Systems.GameSystem
	{
		public Systems.GameSystem System { get; private set; }

		protected GameSystemEvent(TSystem system)
		{
			System = system;
		}
	}

	/// <summary>
	/// Fired after a <see cref="GameSystem"/> has been enabled.
	/// </summary>
	public class GameSystemEnabledEvent<TSystem> : GameSystemEvent<TSystem> where TSystem : Systems.GameSystem
	{
		internal GameSystemEnabledEvent(TSystem system)
			: base(system) {  }
	}

	/// <summary>
	/// Fired after a <see cref="GameSystem"/> has been disabled.
	/// This is also fired when a GameSystem has errored.
	/// </summary>
	public class GameSystemDisabledEvent<TSystem> : GameSystemEvent<TSystem> where TSystem : Systems.GameSystem
	{
		internal GameSystemDisabledEvent(TSystem system)
			: base(system) {  }
	}
}

