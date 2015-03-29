using OtherEngine.Core.Events;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core
{
	public class Game
	{
		public CoreSystemHandler Systems { get; private set; }
		public CoreEventRegistry Events { get; private set; }

		public Game()
		{
			Systems = new CoreSystemHandler(this);
			Events = new CoreEventRegistry(this);

			Events.AutoRegister(typeof(IEventListener<>));
			Events.AutoRegister(typeof(ISystemWatcher));
			Events.AutoRegister(typeof(IComponentWatcher<>));
		}
	}
}

