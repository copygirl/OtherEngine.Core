using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Events;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Controllers
{
	[AutoEnable]
	public class ComponentTrackerController : Controller
	{
		#region GetTrackingControllers

		/// <summary>
		/// Gets an array containing all controllers
		/// currently tracking the specified component type.
		/// </summary>
		public Controller[] GetTrackingControllers(Type componentType)
		{
			if (componentType == null)
				throw new ArgumentNullException("componentType");
			if (componentType.IsSubclassOf(typeof(Component)))
				throw new ArgumentException(string.Format(
					"{0} is not a Component", componentType), "componentType");
			if (componentType.IsAbstract)
				throw new ArgumentException(string.Format(
					"{0} is abstract", componentType), "componentType");

			var controllers = Game.Components.GetContainer(componentType)
				?.Get<TrackedEntitiesComponent>()?.Controllers;

			return controllers?.ToArray() ?? new Controller[0];
		}

		/// <summary>
		/// Gets a readonly collection of all controllers
		/// currently tracking the specified component type.
		/// </summary>
		public Controller[] GetTrackingControllers<TComponent>()
			where TComponent : Component
		{
			return GetTrackingControllers(typeof(TComponent));
		}

		#endregion

		#region Controller enabling / disabling

		[SubscribeEvent]
		void OnControllerEnabled(ControllerEnabledEvent ev)
		{
			var controllerContainer = Game.Controllers.GetContainer(ev.Controller);

			var tracking = controllerContainer.Get<TrackingPropertiesComponent>();
			if (tracking == null)
				controllerContainer.Add(tracking = BuildTrackingProperties(ev.Controller));

			foreach (var setter in tracking.Setters) {
				var componentType = setter.Key;
				var componentContainer = Game.Components.GetContainer(componentType);

				var trackedComponent = componentContainer.GetOrCreate<TrackedEntitiesComponent>();
				trackedComponent.Controllers.Add(ev.Controller);

				setter.Value(trackedComponent.EntitiesReadonly);
			}
		}

		TrackingPropertiesComponent BuildTrackingProperties(Controller controller)
		{
			var component = new TrackingPropertiesComponent();

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var members = controller.GetType().GetMembersWithAttribute<PropertyInfo, TrackComponentAttribute>(flags);

			foreach (var pair in members) {
				var trackedComponentType = pair.Attribute.ComponentType;
				var setter = pair.Member.MakePropertySetter<IReadOnlyCollection<Entity>>(controller, true);
				component.Setters.Add(trackedComponentType, setter);
			}

			return component;
		}

		[SubscribeEvent]
		void OnControllerDisabled(ControllerDisabledEvent ev)
		{
			var controllerContainer = Game.Controllers.GetContainer(ev.Controller);
			var tracking = controllerContainer.GetOrThrow<TrackingPropertiesComponent>();

			foreach (var setter in tracking.Setters) {
				var componentType = setter.Key;
				var componentContainer = Game.Components.GetContainer(componentType);

				var trackedComponent = componentContainer.GetOrThrow<TrackedEntitiesComponent>();
				trackedComponent.Controllers.Remove(ev.Controller);
				if (trackedComponent.Controllers.Count <= 0)
					componentContainer.Remove(trackedComponent);

				setter.Value(null);
			}
		}

		#endregion

		#region Component added / removed

		[SubscribeEvent]
		void OnComponentAdded(ComponentAddedEvent ev)
		{
			var componentContainer = Game.Components.GetContainer(ev.Component);
			var tracked = componentContainer.Get<TrackedEntitiesComponent>();
			if (tracked != null)
				tracked.Entities.Add(ev.Entity);
		}

		[SubscribeEvent]
		void OnComponentRemoved(ComponentRemovedEvent ev)
		{
			var componentContainer = Game.Components.GetContainer(ev.Component);
			var tracked = componentContainer.Get<TrackedEntitiesComponent>();
			if (tracked != null)
				tracked.Entities.Remove(ev.Entity);
		}

		#endregion


		#region Component definitions

		class TrackingPropertiesComponent : Component
		{
			public Dictionary<Type, Action<IReadOnlyCollection<Entity>>> Setters { get; }
				= new Dictionary<Type, Action<IReadOnlyCollection<Entity>>>();
		}

		/// <summary>
		/// Attached to component containers when at least
		/// one controller is tracking the component type.
		/// </summary>
		class TrackedEntitiesComponent : Component
		{
			/// <summary>
			/// Gets the controllers currently tracking this component type.
			/// </summary>
			public HashSet<Controller> Controllers { get; } = new HashSet<Controller>();

			/// <summary>
			/// Gets the 
			/// </summary>
			public HashSet<Entity> Entities { get; } = new HashSet<Entity>();

			/// <summary>
			/// Gets an readonly collection of the Entities set.
			/// This is what the controllers' tracking properties are set to.
			/// </summary>
			public IReadOnlyCollection<Entity> EntitiesReadonly { get; private set; }


			public TrackedEntitiesComponent()
			{
				EntitiesReadonly = new ReadOnlyCollectionWrapper<Entity>(Entities);
			}
		}

		#endregion
	}
}

