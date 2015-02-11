using System;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core
{
	public class Game
	{
		/// <summary>
		/// Returns the game's GameComponents which handles events regarding components.
		/// </summary>
		public GameComponents Components { get; private set; }

		/// <summary>
		/// Returns the game's GameSystemCollection, which holds all currently loaded GameSystem.
		/// </summary>
		public GameSystemCollection Systems { get; private set; }

		public Game()
		{
			Components = new GameComponents(this);
			Systems = new GameSystemCollection(this);
		}
	}
}

