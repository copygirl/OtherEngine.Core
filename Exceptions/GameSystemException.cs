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
}

