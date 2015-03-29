using System.Collections.Generic;
using System.Text.RegularExpressions;
using OtherEngine.Core.Events;

namespace OtherEngine.Core
{
	/// <summary>
	/// Takes care of the game's logic, one piece at a time, by updating
	/// the components of entities, handling and firing events.
	/// Can be enabled, disabled and possibly suspended during runtime.
	/// </summary>
	public abstract class GameSystem
	{
		private static readonly Regex _matchTrailingSystem = new Regex("System$");

		public string Name { get; private set; }

		public Game Game { get; internal set; }
		public GameSystemState State { get; internal set; }
		
		public bool IsRunning { get { return (State >= GameSystemState.Running); } }
		public bool IsEnabled { get { return (State >= GameSystemState.Suspended); } }
		public bool IsSuspended { get { return (State == GameSystemState.Suspended); } }
		public bool IsDisabled { get { return (State <= GameSystemState.Disabled); } }
		public bool IsErrored { get { return (State == GameSystemState.Errored); } }

		protected internal virtual void OnEnabled() {  }
		protected internal virtual void OnDisabled() {  }

		protected GameSystem()
		{
			Name = _matchTrailingSystem.Replace(GetType().Name, "");
		}

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
}

