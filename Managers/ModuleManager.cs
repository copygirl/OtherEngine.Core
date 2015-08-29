using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Managers
{
	/// <summary>
	/// Handles loading modules and initializing
	/// their components, controllers and events.
	/// </summary>
	public class ModuleManager : Manager
	{
		internal ModuleManager(Game game) : base(game) {  }


		/// <summary>
		/// Loads the module contained in this assembly.
		/// Causes controllers with the AutoEnable attribute to be enabled.
		/// Throws an exception if the assembly doesn't contain any components,
		/// controllers or events.
		/// </summary>
		public void Load(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			var componentTypes = new List<Type>();
			var controllerTypes = new List<Type>();
			var eventTypes = new List<Type>();

			foreach (var type in assembly.GetTypes()) {
				if (type.IsSubclassOf(typeof(Component))) componentTypes.Add(type);
				if (type.IsSubclassOf(typeof(Controller))) controllerTypes.Add(type);
				if (type.IsSubclassOf(typeof(Event))) eventTypes.Add(type);

				ValidatedAttribute.ValidateAll(type);
			}

			if ((componentTypes.Count + controllerTypes.Count + eventTypes.Count) <= 0)
				throw new ArgumentException(string.Format(
					"{0} doesn't contain component, controller or event types", assembly), "assembly");

			Game.Events.DelayEvents();

			Game.Components.OnModuleLoad(componentTypes);
			Game.Controllers.OnModuleLoad(controllerTypes);
			Game.Events.OnModuleLoad(eventTypes);

			Game.Events.FireDelayedEvents();

			Game.Components.OnModuleLoaded();
			Game.Controllers.OnModuleLoaded();
			Game.Events.OnModuleLoaded();

			// Enable all controllers with the AutoEnable attribute.
			foreach (var controllerType in controllerTypes)
				if (controllerType.GetAttribute<AutoEnableAttribute>() != null)
					Game.Controllers.Enable(Game.Controllers.Get(controllerType));
		}

		// TODO: Create module entities.
	}
}

