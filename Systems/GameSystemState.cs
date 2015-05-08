namespace OtherEngine.Core.Systems
{
	public enum GameSystemState
	{
		/// <summary>
		/// GameSystem state is unknown, possibly not
		/// attached to a <see cref="Game"/> instance.
		/// </summary>
		Unknown,

		/// <summary>
		/// GameSystem has errored, though it can be reenabled.
		/// </summary>
		Errored,

		/// <summary>
		/// GameSystem is disabled, it will not receive events.
		/// </summary>
		Disabled,

		/// <summary>
		/// GameSystem is enabled but should be passive.
		/// Events are still received to keep things up-to-date.
		/// </summary>
		Suspended,

		/// <summary>
		/// GameSystem is doing all sorts of things.
		/// </summary>
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
