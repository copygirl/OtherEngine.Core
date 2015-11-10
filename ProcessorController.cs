using System;
using System.Collections.Generic;
using OtherEngine.Utility;

namespace OtherEngine.Core
{
	public class ProcessorController : IReadOnlyCollection<ProcessorContainer>
	{
		#region Private fields

		readonly Dictionary<Type, ProcessorContainer> _processorsByType =
			new Dictionary<Type, ProcessorContainer>();

		readonly Dictionary<string, ProcessorContainer> _processorsByIdentifier =
			new Dictionary<string, ProcessorContainer>();

		readonly Game _game;

		#endregion

		internal ProcessorController(Game game) { _game = game; }


		internal void OnModuleLoaded(ModuleContainer module)
		{
			foreach (var processor in module.Processors) {
				_processorsByType.Add(processor.ProcessorType, processor);
				_processorsByIdentifier.Add(processor.Identifier, processor);
			}
		}

		internal void Enable()
		{
			foreach (var processor in this)
				if (processor.AutoEnable && (processor.State != ProcessorState.Enabled))
					processor.Enable();
		}


		#region GetContainer methods

		/// <summary> Returns the processor container for type
		///           TProcessor, or null if it's not loaded. </summary>
		public ProcessorContainer GetContainer<TProcessor>()
			where TProcessor : IProcessor
		{
			return GetContainer(typeof(TProcessor));
		}

		/// <summary> Returns the processor container for the specified
		///           processor type, or null if it's not loaded. </summary>
		public ProcessorContainer GetContainer(Type processorType)
		{
			if (processorType == null)
				throw new ArgumentNullException("processorType");
			if (!typeof(IProcessor).IsAssignableFrom(processorType))
				throw new ArgumentException(string.Format(
					"{0} is not an IProcessor", processorType), "processorType");
			
			return _processorsByType.GetOrDefault(processorType);
		}

		/// <summary> Returns the processor container matching the
		///           specified identifier, or null if it's not loaded. </summary>
		public ProcessorContainer GetContainer(string identifier)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			if (identifier.IndexOf(':') < 0)
				throw new ArgumentException(string.Format(
					"\"{0}\" doesn't look like a valid processor identifier", identifier), "identifier");

			return _processorsByIdentifier.GetOrDefault(identifier);
		}

		#endregion

		#region GetInstance methods

		/// <summary> Returns the processor instance of type TProcessor,
		///           or null if it's not loaded or enabled. </summary>
		public TProcessor GetInstance<TProcessor>()
			where TProcessor : IProcessor
		{
			return (TProcessor)GetInstance(typeof(TProcessor));
		}

		/// <summary> Returns the processor instance of the specified
		///           processor type, or null if it's not loaded or enabled. </summary>
		public IProcessor GetInstance(Type processorType)
		{
			var container = GetContainer(processorType);
			return ((container?.State == ProcessorState.Enabled) ? container.Instance : null);
		}

		/// <summary> Returns the processor instance matching the specified
		///           identifier, or null if it's not loaded or enabled. </summary>
		public IProcessor GetInstance(string identifier)
		{
			var container = GetContainer(identifier);
			return ((container?.State == ProcessorState.Enabled) ? container.Instance : null);
		}

		#endregion


		#region IReadOnlyCollection<ProcessorContainer> implementation

		public int Count { get { return _processorsByType.Count; } }

		public IEnumerator<ProcessorContainer> GetEnumerator()
		{
			return _processorsByType.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

