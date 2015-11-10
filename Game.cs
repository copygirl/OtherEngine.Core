using OtherEngine.ES;
using System;

namespace OtherEngine.Core
{
	public class Game
	{
		/// <summary> Gets the main GameTimeline of this game instance.
		///           This contains all information about the current
		///           (and possible past and future) game state. </summary>
		public GameTimeline Timeline { get; private set; }

		/// <summary> Gets the module controller of this game instance. </summary>
		public ModuleController Modules { get; private set; }

		/// <summary> Gets the processor controller of this game instance. </summary>
		public ProcessorController Processors { get; private set; }

		/// <summary> Gets the thread controller of this game instance. </summary>
		public ThreadController Threads { get; private set; }


		/// <summary> Gets the current state of the game. </summary>
		public GameState State { get; private set; }


		public Game()
		{
			Timeline = new GameTimeline();
			Modules = new ModuleController(this);
			Processors = new ProcessorController(this);
			Threads = new ThreadController(this);

			State = GameState.Initializing;
			Modules.Load(typeof(Game).Assembly);
		}


		public void Start()
		{
			if (State != GameState.Initializing)
				throw new InvalidOperationException("Game isn't initializing");
			
			State = GameState.Running;
			Processors.Enable();
			Threads.Start();
		}

		public void Stop()
		{
			if (State != GameState.Running)
				throw new InvalidOperationException("Game isn't running");
			
			State = GameState.Stopped;
		}
	}

	public enum GameState
	{
		Invalid,
		Initializing,
		Running,
		Stopped
	}
}

