using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OtherEngine.ES;

namespace OtherEngine.Core
{
	public class ThreadContainer : IReadOnlyCollection<ProcessorContainer>
	{
		readonly List<ProcessorContainer> _processors = new List<ProcessorContainer>();

		readonly LinkedList<ProcessorHelper> _processorQueue = new LinkedList<ProcessorHelper>();

		readonly Stopwatch _stopwatch = new Stopwatch();


		public GameTime DefaultInterval { get; set; }

		public bool DeltaFixed { get; set; }


		internal ThreadContainer()
		{
			DefaultInterval = GameTime.FromSeconds(1.0 / 30);
			DeltaFixed = true;
		}


		#region IThread implementation

		public void Start(IEnumerable<ProcessorContainer> processors)
		{
			_stopwatch.Start();

			foreach (var processor in processors)
				_processorQueue.AddLast(new ProcessorHelper(processor){
					// Set previous value so delta time won't be
					// 0 the first time the processors are ticked.
					Previous = -(processor.Interval ?? DefaultInterval)
				});
		}

		public void Process(Game game)
		{
			while (game.State == GameState.Running) {
				var node = _processorQueue.First;
				var helper = node.Value;
				var processor = helper.Container;

				// Sleep until processor needs to tick, if necessary.
				var sleep = helper.Next - (GameTime)_stopwatch.Elapsed;
				if (sleep > GameTime.Zero)
					Thread.Sleep((TimeSpan)sleep);

				// If processor isn't running at a fixed interval, use exact time.
				var current = (DeltaFixed ? helper.Next : (GameTime)_stopwatch.Elapsed);

				if (processor.State == ProcessorState.Enabled)
					processor.Instance.Tick(game, current, current - helper.Previous);

				helper.Next = current + (processor.Interval ?? DefaultInterval);
				helper.Previous = current;

				// Re-insert processor into processor list.
				_processorQueue.Remove(node);
				for (var n = _processorQueue.Last; (n != null); n = n.Previous)
					if (n.Value.Next <= helper.Next) {
						_processorQueue.AddAfter(n, helper);
						break;
					}
				if (node.List == null)
					_processorQueue.AddFirst(node);
			}
		}

		#endregion


		#region IReadOnlyCollection<ProcessorContainer> implementation

		public int Count { get { return _processors.Count; } }

		public IEnumerator<ProcessorContainer> GetEnumerator()
		{
			return _processors.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


		#region ProcessorHelper class

		protected class ProcessorHelper
		{
			public ProcessorContainer Container { get; private set; }

			public GameTime Previous { get; set; }

			public GameTime Next { get; set; }


			public ProcessorHelper(ProcessorContainer container)
			{
				Container = container;
			}
		}

		#endregion
	}
}

