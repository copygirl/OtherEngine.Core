
namespace OtherEngine.Core.Managers
{
	public abstract class Manager
	{
		protected Game Game { get; private set; }

		internal Manager(Game game)
		{
			Game = game;
		}
	}
}

