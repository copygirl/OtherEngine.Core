using System;
using System.Text.RegularExpressions;

namespace OtherEngine.Core.Systems
{
	/// <summary>
	/// Takes care of the game's logic, one piece at a time, by updating
	/// the components of entities, handling and firing events.
	/// Can be enabled, disabled and possibly suspended during runtime.
	/// </summary>
	public abstract class GameSystem
	{
		static readonly Regex _matchTrailingSystem = new Regex("System$");

		/// <summary>
		/// Gets the "friendly" name of this GameSystem without the "System" suffix.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the <see cref="Game"/> instance this GameSystem is attached to.
		/// </summary>
		/// <value>The game.</value>
		public Game Game { get; internal set; }

		/// <summary>
		/// Gets the <see cref="GameSystemState"/> of the GameSystem.
		/// </summary>
		public GameSystemState State { get; internal set; }


		/// <summary>
		/// Creates a new GameSystem instance.
		/// Note that you should create GameSystems using
		/// <c>Game.Systems.Get&lt;TSystem&gt;</c>
		/// for them to be part of the <see cref="Game"/> instance.
		/// </summary>
		protected GameSystem()
		{
			Name = GetName(GetType());
		}


		#region Enabling / disabling

		/// <summary>
		/// Enables this GameSystem, causing it to start receiving events.
		/// </summary>
		public void Enable() {
			if (Game == null)
				throw new InvalidOperationException(String.Format(
					"{0} is not attached to a Game.", this));
			Game.Systems.Enable(this);
		}

		/// <summary>
		/// Disables this GameSystem, causing it to stop receiving events.
		/// </summary>
		public void Disable() {
			if (Game == null)
				throw new InvalidOperationException(String.Format(
					"{0} is not attached to a Game.", this));
			Game.Systems.Disable(this);
		}


		/// <summary>
		/// Called when the GameSystem is enabled, allows for settings necessary things up.
		/// </summary>
		protected internal virtual void OnEnabled() {  }

		/// <summary>
		/// Called when the GameSystem is disabled, allows for cleaning things up.
		/// </summary>
		protected internal virtual void OnDisabled() {  }

		#endregion

		#region ToString / name related

		static string GetStateString(GameSystemState state)
		{
			switch (state) {
				case GameSystemState.Running: return " [Run]";
				case GameSystemState.Suspended: return " [Sus]";
				case GameSystemState.Disabled: return " [Dis]";
				case GameSystemState.Errored: return " [Err]";
				default: return " [???]";
			}
		}
		static string ToString(string name, string stateString)
		{
			return string.Format("[System: {0}{1}]", name, stateString);
		}

		public sealed override string ToString()
		{
			return ToString(Name, GetStateString(State));
		}

		/// <summary>
		/// Returns the name of a GameSystem type, same as the instance version.
		/// </summary>
		public static string GetName(Type systemType)
		{
			if (systemType == null)
				throw new ArgumentNullException("systemType");
			if (!systemType.IsSubclassOf(typeof(GameSystem)))
				throw new ArgumentException(String.Format(
					"{0} is not a GameSystem", systemType), "systemType");

			return _matchTrailingSystem.Replace(systemType.Name, "");
		}

		/// <summary>
		/// Returns a string representation of the GameSystem type, same as the instance version.
		/// </summary>
		public static string ToString(Type systemType)
		{
			return ToString(GetName(systemType), "");
		}

		#endregion
	}
}

