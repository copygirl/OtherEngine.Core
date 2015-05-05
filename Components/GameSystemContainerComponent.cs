using System;
using OtherEngine.Core.Data;

namespace OtherEngine.Core.Components
{
	public class GameSystemContainerComponent : GameComponent
	{
		public Type SystemType { get; private set; }
		public GameSystem System { get; internal set; }

		internal GameSystemContainerComponent(Type systemType)
		{
			SystemType = systemType;
		}
	}
}

