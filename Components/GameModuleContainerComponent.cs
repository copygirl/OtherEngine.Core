using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Data;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Component attached to <see cref="GameData"/> objects representing module containers,
	/// which can be looked up using <see cref="CoreModuleHandler.GetContainer"/>.
	/// </summary>
	public class GameModuleContainerComponent : GameComponent
	{
		public Assembly Assembly { get; private set; }

		public IEnumerable<Type> ComponentTypes { get; private set; }
		public IEnumerable<Type> SystemTypes { get; private set; }
		public IEnumerable<Type> EventTypes { get; private set; }

		public bool Loaded { get; internal set; }

		internal GameModuleContainerComponent(
			Assembly assembly, IEnumerable<Type> componentTypes,
			IEnumerable<Type> systemTypes, IEnumerable<Type> eventTypes)
		{
			Assembly = assembly;

			ComponentTypes = componentTypes;
			SystemTypes = systemTypes;
			EventTypes = eventTypes;
		}
	}
}

