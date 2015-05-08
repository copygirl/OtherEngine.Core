using System;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Exceptions
{
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
				throw new Exception(string.Format("Game system state == desiredState ({0})", system.State));
		}
		
		static string GetDefaultMessage(GameSystem system, GameSystemState desiredState)
		{
			string symbol = (desiredState > system.State) ? ">=" : "<=";
			return string.Format("{0} is {1}, needs to be {2} {3}", system, system.State, symbol, desiredState);
		}
	}
}
