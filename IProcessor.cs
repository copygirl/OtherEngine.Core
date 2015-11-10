using OtherEngine.ES;

namespace OtherEngine.Core
{
	/// <summary> Interface for processors, each of which take care of one small
	///           piece of game logic. Loaded when a module (assembly) is loaded.
	///           
	///           Requires a public parameter-less constructor.
	///           Can be enabled automatically with the [AutoEnabled] attribute. </summary>
	public interface IProcessor
	{
		/// <summary> Called every interval if this processor is enabled.
		///           Can be controlled using the [ProcessorInterval] attribute. </summary>
		/// <param name="game"> The game instance the processor is running in. </param>
		/// <param name="current"> The current GameTime for this tick. </param>
		/// <param name="delta"> The time since the last tick. </param>
		void Tick(Game game, GameTime current, GameTime delta);
	}
}

