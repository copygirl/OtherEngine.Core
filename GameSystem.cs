using System.Text.RegularExpressions;
using System.Collections.Generic;
using OtherEngine.Core.Events;

namespace OtherEngine.Core
{
	/// <summary>
	/// Takes care of the game's logic, one piece at a time,
	/// by updating the components of entities, handling and firing events.
	/// Can be enabled, disabled and possibly suspended.
	/// </summary>
	public abstract class GameSystem
	{
		private static readonly Regex _matchTrailingSystem = new Regex("System$");

		// TODO: Unregister from events when system is disabled or errors.
		internal ICollection<GameEvent> _events = new HashSet<GameEvent>();

		public string Name { get; private set; }

		public Game Game { get; internal set; }
		public GameSystemState State { get; internal set; }

		protected GameSystem()
		{
			Name = _matchTrailingSystem.Replace(GetType().Name, "");
		}

		protected internal virtual void OnEnabled() {  }
		protected internal virtual void OnDisabled() {  }

		public sealed override string ToString()
		{
			return string.Format("[System: {0} ({1})]", Name, State);
		}
	}

	public enum GameSystemState
	{
		Unknown,
		Errored,
		Disabled,
		Suspended,
		Enabled
	}
}

