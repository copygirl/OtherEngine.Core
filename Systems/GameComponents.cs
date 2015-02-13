using System.Collections.Generic;
using System.Linq;
using OtherEngine.Core.Collections;

namespace OtherEngine.Core.Systems
{
	/// <summary>
	/// Container class for the game's component types.
	/// Allows watching for when components get added or removed from entities.
	/// </summary>
	public class GameComponents
	{
		private readonly ComponentHandlerCollection _handlers = new ComponentHandlerCollection();

		private readonly Game _game;

		public GameComponents(Game game)
		{
			_game = game;
		}

		public ComponentHandler<T> Get<T>() where T : IGameComponent
		{
			return (ComponentHandler<T>)_handlers.Get<T>(TypedGetBehavior.CreateAndAdd);
		}

		internal void OnAdded<T>(GameEntity entity, T component) where T : IGameComponent
		{
			((ComponentHandler<T>)_handlers.Get<T>(TypedGetBehavior.Create)).OnComponentAdded(entity, component);
		}
		internal void OnRemoved<T>(GameEntity entity, T component) where T : IGameComponent
		{
			((ComponentHandler<T>)_handlers.Get<T>(TypedGetBehavior.Create)).OnComponentRemoved(entity, component);
		}


		public class ComponentHandler<T> where T : IGameComponent
		{
			private readonly ICollection<GameEntity> _entities = new HashSet<GameEntity>();

			public IEnumerable<GameEntity> Entities { get { return _entities.Select(x => x); } }

			internal ComponentHandler() {  }

			internal void OnComponentAdded(GameEntity entity, T component)
			{

			}
			internal void OnComponentRemoved(GameEntity entity, T component)
			{

			}
		}

		public class ComponentHandlerCollection : TypedDefaultCollection<IGameComponent, object>
		{
			protected override object NewValue<T>()
			{
				return new ComponentHandler<T>();
			}
		}
	}
}

