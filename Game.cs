using OtherEngine.Core.Managers;
using OtherEngine.Core.Components;

namespace OtherEngine.Core
{
	public class Game
	{
		public ComponentManager Components { get; private set; }

		public ControllerManager Controllers { get; private set; }

		public EventManager Events { get; private set; }

		public ModuleManager Modules { get; private set; }


		public Entity Hierarchy { get; private set; }


		public Game()
		{
			Components = new ComponentManager(this);
			Controllers = new ControllerManager(this);
			Events = new EventManager(this);
			Modules = new ModuleManager(this);

			Hierarchy = new Entity(this);

			Modules.Load(typeof(Game).Assembly);

			Hierarchy.Add(new TypeComponent { Value = "Game" });
		}
	}
}

