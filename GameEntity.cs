using System;
using OtherEngine.Core.Collections;

namespace OtherEngine.Core
{
	public sealed class GameEntity
	{
		public Game Game { get; private set; }

		public ComponentCollection Components { get; private set; }

		public GameEntity(Game game)
		{
			Game = game;

			Components = new ComponentCollection(this);
		}
	}
}

