using System;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Exceptions
{
	/// <summary>
	/// Thrown when there is a problem with a specific <see cref="GameSystem"/>.
	/// May cause said GameSystem to be disabled and go into Errored state.
	/// </summary>
	public class GameSystemException : Exception
	{
		public Type SystemType { get; private set; }
		public GameSystem System { get; private set; } // May be null.

		private GameSystemException(Type type, GameSystem system, string message, Exception innerException)
			: base(message, innerException)
		{
			SystemType = type;
			System = system;
		}

		public GameSystemException(GameSystem system, string message, Exception innerException = null)
			: this(ThrowIfNull(system, "system").GetType(), system, message, innerException) {  }
		
		public GameSystemException(Type type, string message, Exception innerException = null)
			: this(ThrowIfNull(type, "type"), null, message, innerException) {  }


		static T ThrowIfNull<T>(T value, string paramName)
		{
			if (value == null)
				throw new ArgumentNullException("paramName");
			return value;
		}
	}
}

