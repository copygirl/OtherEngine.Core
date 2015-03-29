using System;

namespace OtherEngine.Core.Exceptions
{
	public class GameSystemException : Exception
	{
		public Type SystemType { get; private set; }
		public GameSystem System { get; private set; }

		public GameSystemException(Type systemType, string message, Exception innerException = null)
			: base(message, innerException)
		{
			if (systemType == null)
				throw new ArgumentNullException("systemType");
			if (!systemType.IsSubclassOf(typeof(GameSystem)))
				throw new ArgumentException("systemType is not a subclass of GameSystem", "systemType");
			SystemType = systemType;
		}

		public GameSystemException(GameSystem system, string message, Exception innerException = null)
			: base(message, innerException)
		{
			if (system == null)
				throw new ArgumentNullException("system");
			SystemType = system.GetType();
			System = system;
		}
	}

	public class GameSystemStateException : GameSystemException
	{
		public GameSystemState State { get; private set; }
		public GameSystemState MinimumState { get; private set; }

		public GameSystemStateException(GameSystem system, GameSystemState minimumState, string message) : base(system, message)
		{
			State = system.State;
			MinimumState = minimumState;
		}
		public GameSystemStateException(GameSystem system, GameSystemState minimumState)
			: base(system, minimumState, GetDefaultMessage(system, minimumState)) {  }

		private static string GetDefaultMessage(GameSystem system, GameSystemState minimumState)
		{
			return String.Format("{0} is {1}, needs to be {2}", system, system.State, minimumState);
		}
	}
}

