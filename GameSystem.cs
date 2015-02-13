namespace OtherEngine.Core
{
	/// <summary>
	/// Takes care of the game's logic, one piece at a time,
	/// by updating the components of entities, handling and firing events.
	/// Can be enabled, disabled and possibly suspended.
	/// </summary>
	public abstract class GameSystem
	{
		protected internal Game Game { get; internal set; }

		public GameSystemState State { get; internal set; }

		protected internal virtual void OnEnabled() {  }
		protected internal virtual void OnDisabled() {  }
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

