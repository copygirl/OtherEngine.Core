using System;
using OtherEngine.Core.Data;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Component attached to <see cref="GameData"/> objects representing
	/// containers of <see cref="GameSystem"/>s, which can be looked up
	/// using <see cref="CoreSystemHandler.GetContainer"/>.
	/// </summary>
	public class GameSystemContainerComponent : GameComponent
	{
		public Type SystemType { get; private set; }
		public Systems.GameSystem System { get; internal set; }
		public bool ConstructorThrewException { get; internal set; }

		internal GameSystemContainerComponent(Type systemType)
		{
			SystemType = systemType;
		}
	}
}

