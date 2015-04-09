using System;

namespace OtherEngine.Core.Exceptions
{
	/// <summary>
	/// Exception which signifies an issue in a specific GameSystem.
	/// When thrown, may cause said system to be disabled and go into Errored state.
	/// </summary>
	public class GameSystemException : Exception
	{
		public GameSystem System { get; private set; }

		public GameSystemException(GameSystem system, string message, Exception innerException = null)
			: base(message, innerException)
		{
			if (system == null)
				throw new ArgumentNullException("system");
			System = system;
		}
	}

	/// <summary>
	/// Helper exception, ideally thrown when another system is required
	/// to be in a specific state before some action can be executed.
	/// </summary>
	public class GameSystemStateException : Exception
	{
		public GameSystem System { get; private set; }

		public GameSystemStateException(GameSystem system, string message)
			: base(message)
		{
			System = system;
		}

		public GameSystemStateException(GameSystem system, GameSystemState desiredState)
			: this(system, GetDefaultMessage(system, desiredState))
		{
			if (system.State == desiredState)
				throw new Exception(String.Format("Game system state == desiredState ({0})", system.State));
		}
		
		private static string GetDefaultMessage(GameSystem system, GameSystemState desiredState)
		{
			string symbol = (desiredState > system.State) ? ">=" : "<=";
			return string.Format("{0} is {1}, needs to be {2} {3}", system, system.State, symbol, desiredState);
		}
	}
}

