using System.Collections.Generic;
using System.Linq;
using OtherEngine.Utility;

namespace OtherEngine.Core
{
	public class ThreadController
	{
		readonly Dictionary<string, ThreadContainer> _threads =
			new Dictionary<string, ThreadContainer>();

		readonly Game _game;


		internal ThreadController(Game game)
		{
			_game = game;
			_threads.Add(MainThread, new ThreadContainer(game, MainThread, true));
		}


		internal void OnModuleLoaded(ModuleContainer module)
		{
			var processorsByThread = module.Processors
				.GroupBy(proc => proc.ThreadIdentifier ?? DefaultThread);
			
			foreach (var threadGroup in processorsByThread) {
				var thread = _threads.GetOrAdd(threadGroup.Key, id => new ThreadContainer(_game, id));
				foreach (var processor in threadGroup)
					thread.Add(processor);
			}
		}

		internal void Start()
		{
			foreach (var thread in _threads.Values)
				if (!thread.IsMainThread)
					thread.Start();
			
			Find(MainThread).Start();
		}


		public ThreadContainer Find(string identifier)
		{
			return _threads.GetOrDefault(identifier);
		}


		#region IReadOnlyCollection<ThreadContainer> implementation

		public int Count { get { return _threads.Count; } }

		public IEnumerator<ThreadContainer> GetEnumerator()
		{
			return _threads.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

