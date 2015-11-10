using System;
using System.Text.RegularExpressions;
using OtherEngine.Core.Attributes;
using OtherEngine.ES;
using OtherEngine.Utility.Attributes;

namespace OtherEngine.Core
{
	public class ProcessorContainer
	{
		static readonly Regex _identifierRegex = new Regex("Processor$");


		#region Public properties

		/// <summary> Gets the module this processor belongs to. </summary>
		public ModuleContainer Module { get; private set; }

		/// <summary> Gets the game instance this processor was created for. </summary>
		public Game Game { get { return Module.Game; } }

		/// <summary> Gets the type of this processor. </summary>
		public Type ProcessorType { get; private set; }

		/// <summary> Gets the identifier of this processor,
		///           for example "OtherEngine.Core:Window". </summary>
		public string Identifier { get; private set; }


		/// <summary> Gets the instance of this processor, or null if uninitialized. </summary>
		public IProcessor Instance { get; private set; }

		/// <summary> Gets the current state of this processor </summary>
		public ProcessorState State { get; private set; }


		/// <summary> Gets whether the processor should be enabled
		///           automatically when the module is loaded. </summary>
		public bool AutoEnable { get; private set; }

		/// <summary> Gets the identifier for the thread this processor is meant
		///           to be run in, or null if the default thread should be used. </summary>
		public string ThreadIdentifier { get; private set; }

		/// <summary> Gets the interval at which the processor is supposed
		///           to run, or null if it should use the thread's default. </summary>
		public GameTime? Interval { get; private set; }

		#endregion


		internal ProcessorContainer(ModuleContainer module, Type processorType)
		{
			Module = module;
			ProcessorType = processorType;
			Identifier = Module.Name + ":" + _identifierRegex.Replace(processorType.Name, "");

			AutoEnable = (processorType.GetAttribute<AutoEnableAttribute>() != null);
			ThreadIdentifier = processorType.GetAttribute<ThreadAttribute>()?.Identifier;
			var intervalAttribute = processorType.GetAttribute<IntervalAttribute>();
			Interval = intervalAttribute?.Interval;
		}


		#region Enabling and disabling

		public void Enable()
		{
			if (Game.State != GameState.Running)
				throw new InvalidOperationException(
					"Can't enable processors while game isn't running");
			if (State == ProcessorState.Enabled)
				throw new InvalidOperationException(string.Format(
					"{0} is already enabled", this));

			State = ProcessorState.Enabled;
			if (Instance == null)
				Instance = (IProcessor)Activator.CreateInstance(ProcessorType);

		}

		public void Disable()
		{
			if (State != ProcessorState.Enabled)
				throw new InvalidOperationException(string.Format(
					"{0} is not enabled", this));

			State = ProcessorState.Disabled;
		}

		#endregion


		public override string ToString()
		{
			return string.Format("[Processor {0}]", Identifier);
		}
	}

	public enum ProcessorState
	{
		Invalid,
		Errored,
		Disabled,
		Enabled
	}
}

