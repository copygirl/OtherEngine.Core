using System;

namespace OtherEngine.Core.Components
{
	public class GameSystemContainerComponent : IGameComponent
	{
		public Type SystemType { get; private set; }
		public GameSystem System { get; internal set; }

		internal GameSystemContainerComponent(Type systemType)
		{
			SystemType = systemType;
		}
	}
}

