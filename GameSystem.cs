using System.Text.RegularExpressions;
using OtherEngine.Core.Data;

namespace OtherEngine.Core
{
	/// <summary>
	/// Takes care of the game's logic, one piece at a time, by updating
	/// the components of entities, handling and firing events.
	/// Can be enabled, disabled and possibly suspended during runtime.
	/// </summary>
	public abstract class GameSystem
	{
		static readonly Regex _matchTrailingSystem = new Regex("System$");

		public string Name { get; private set; }

		public Game Game { get; internal set; }
		public GameSystemState State { get; internal set; }

		protected GameSystem()
		{
			Name = _matchTrailingSystem.Replace(GetType().Name, "");
		}

		public void Enable() { Game.Systems.Enable(this); }
		public void Disable() { Game.Systems.Disable(this); }

		protected internal virtual void OnEnabled() {  }
		protected internal virtual void OnDisabled() {  }

		public sealed override string ToString()
		{
			return string.Format("[System: {0}{1}]", Name, GetStateString());
		}

		private string GetStateString()
		{
			switch (State) {
				case GameSystemState.Running: return "";
				case GameSystemState.Suspended: return " S";
				case GameSystemState.Disabled: return " X";
				case GameSystemState.Errored: return " E";
				default: return " ?";
			}
		}
	}

	public enum GameSystemState
	{
		Unknown,
		Errored,
		Disabled,
		Suspended,
		Running
	}

	public static class GameSystemStateExtensions
	{
		public static bool IsRunning(this GameSystemState state) { return (state >= GameSystemState.Running); }
		public static bool IsEnabled(this GameSystemState state) { return (state >= GameSystemState.Suspended); }
		public static bool IsSuspended(this GameSystemState state) { return (state == GameSystemState.Suspended); }
		public static bool IsDisabled(this GameSystemState state) { return (state <= GameSystemState.Disabled); }
		public static bool IsErrored(this GameSystemState state) { return (state == GameSystemState.Errored); }
	}
}

