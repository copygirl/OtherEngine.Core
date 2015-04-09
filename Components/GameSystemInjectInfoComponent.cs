using System;
using System.Collections.Generic;
using System.Reflection;

namespace OtherEngine.Core.Components
{
	internal class GameSystemInjectInfoComponent : IGameComponent
	{
		public IDictionary<Type, FieldInfo> Requests { get; private set; }
		public IDictionary<GameSystem, FieldInfo> References { get; private set; }

		public GameSystemInjectInfoComponent()
		{
			Requests = new Dictionary<Type, FieldInfo>();
			References = new Dictionary<GameSystem, FieldInfo>();
		}
	}
}

