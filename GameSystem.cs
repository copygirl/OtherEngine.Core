using System;
using OtherEngine.Core.Utility;
using OtherEngine.Core.Exceptions;

namespace OtherEngine.Core
{
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

