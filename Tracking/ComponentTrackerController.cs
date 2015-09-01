using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Events;
using OtherEngine.Core.Utility;
using System.Linq.Expressions;

namespace OtherEngine.Core.Tracking
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
				?.Get<TrackedEntitiesComponent>()?._controllers;

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

				var trackedComponent = componentContainer.Get<TrackedEntitiesComponent>();
				if (trackedComponent == null) {
					var collectionType = typeof(EntityCollection<>).MakeGenericType(componentType);
					var collection = (EntityCollection)Activator.CreateInstance(collectionType, true);
					trackedComponent = new TrackedEntitiesComponent(collection);
					componentContainer.Add(trackedComponent);
				}
				trackedComponent._controllers.Add(ev.Controller);

				setter.Value(trackedComponent.Entities);
			}
		}

		TrackingPropertiesComponent BuildTrackingProperties(Controller controller)
		{
			var component = new TrackingPropertiesComponent();

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var members = controller.GetType().GetMembersWithAttribute<PropertyInfo, TrackComponentAttribute>(flags);

			foreach (var pair in members) {
				var collectionType = pair.Member.PropertyType;
				var trackedComponentType = collectionType.GetGenericArguments()[0];

				var parameter = Expression.Parameter(typeof(EntityCollection));
				var body = Expression.Call(Expression.Constant(controller), pair.Member.GetSetMethod(true), Expression.Convert(parameter, collectionType));
				var setter = Expression.Lambda<Action<EntityCollection>>(body, parameter).Compile();

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
				trackedComponent._controllers.Remove(ev.Controller);
				if (trackedComponent._controllers.Count <= 0)
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


		#region TrackingPropertiesComponent definition

		class TrackingPropertiesComponent : Component
		{
			public Dictionary<Type, Action<EntityCollection>> Setters { get; }
				= new Dictionary<Type, Action<EntityCollection>>();
		}

		#endregion
	}
}

