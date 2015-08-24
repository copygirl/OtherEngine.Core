using OtherEngine.Core.Managers;

namespace OtherEngine.Core
{
	public class Game
	{
		public ComponentManager Components { get; private set; }

		public ControllerManager Controllers { get; private set; }

		public EventManager Events { get; private set; }

		public ModuleManager Modules { get; private set; }


		public Game()
		{
			Components = new ComponentManager(this);
			Controllers = new ControllerManager(this);
			Events = new EventManager(this);
			Modules = new ModuleManager(this);
		}
	}
}

