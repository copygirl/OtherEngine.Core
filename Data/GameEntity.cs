using System;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Data
{
	/// <summary>
	/// Empty game object which holds a number of <see cref="GameComponent"/>s which can be looked up using their type.
	/// While <see cref="GameData"/> is just a simple container, GameEntities will notify the game whenever components
	/// are added or removed, which allows them to be tracked by <see cref="GameSystem"/>s.
	/// </summary>
	public sealed class GameEntity : GameData
	{
		public Game Game { get; private set; }

		public GameEntity(Game game)
		{
			if (game == null)
				throw new ArgumentNullException("game");
			Game = game;
		}

		public override void Add(GameComponent component)
		{
			base.Add(component);
			Game.OnComponentAdded(this, component);
		}

		public override void Remove(GameComponent component)
		{
			base.Remove(component);
			Game.OnComponentRemoved(this, component);
		}
	}
}

