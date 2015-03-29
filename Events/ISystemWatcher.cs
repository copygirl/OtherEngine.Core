using System;

namespace OtherEngine.Core.Events
{
	public interface ISystemWatcher
	{
		void OnSystemEnabled(GameSystem system);

		void OnSystemDisabled(GameSystem system);

		void OnSystemErrored(GameSystem system);
	}
}

