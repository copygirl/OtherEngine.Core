using System;
using System.Collections.Generic;
using System.Reflection;

namespace OtherEngine.Core.Components
{
	internal class GameSystemEventSubsComponent : IGameComponent
	{
		public IDictionary<Type, Tuple<MethodInfo, Action<IGameEvent>>> EventListeners { get; private set; }

		public GameSystemEventSubsComponent()
		{
			EventListeners = new Dictionary<Type, Tuple<MethodInfo, Action<IGameEvent>>>();
		}
	}
}

