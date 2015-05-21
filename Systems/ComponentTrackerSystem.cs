using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Components;
using OtherEngine.Core.Data;
using OtherEngine.Core.Events;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Systems
{
	/// <summary>
	/// Keeps track of <see cref="GameEntity"/>s with <see cref="GameComponent"/>s
	/// of certain types for other <see cref="GameSystem"/>s, by inserting an
	/// enumerable into those GameSystems.
	/// </summary>
	/// <seealso cref="TrackComponentAttribute"/>
	[AutoEnable]
	public class ComponentTrackerSystem : GameSystem
	{
		readonly TrackingDictionary _trackedComponents = new TrackingDictionary();

		[SubscribeEvent]
		public void OnComponentAdded(GameComponentAddedEvent<GameComponent> ev)
		{
			var container = GetContainer(ev.Component.GetType());
			if (container != null)
				container.EntityCollection.Add(ev.Entity);
		}
		[SubscribeEvent]
		public void OnComponentRemoved(GameComponentRemovedEvent<GameComponent> ev)
		{
			var container = GetContainer(ev.Component.GetType());
			if (container != null)
				container.EntityCollection.Remove(ev.Entity);
		}

		#region System enabling / disabling

		protected internal override void OnEnabled()
		{
			foreach (var system in Game.Systems)
				if (system.State.IsEnabled())
					OnSystemEnabled(system);
		}
		protected internal override void OnDisabled()
		{
			_trackedComponents.Clear();
			foreach (var system in Game.Systems)
				if (system.State.IsEnabled())
					OnSystemDisabled(system);
		}

		[SubscribeEvent]
		public void OnSystemEnabled(GameSystemEnabledEvent<GameSystem> ev)
		{
			OnSystemEnabled(ev.System);
		}
		[SubscribeEvent]
		public void OnSystemDisabled(GameSystemDisabledEvent<GameSystem> ev)
		{
			OnSystemDisabled(ev.System);
		}

		/// <summary>
		/// Called when this or any other <see cref="GameSystem"/> is enabled.
		/// Gets all properties with the <see cref="TrackComponentAttribute"/> and sets
		/// them to an enumerable of all tracked <see cref="GameEntity"/>s with the
		/// requested <see cref="GameComponent"/> type.
		/// </summary>
		void OnSystemEnabled(GameSystem system)
		{
			foreach (var kvp in GetTracking(system))
				kvp.Value.SetValue(system, ContainerAddSystem(kvp.Key, system));
		}
		/// <summary>
		/// Called when this or any other <see cref="GameSystem"/> is disabled.
		/// Gets all properties with the <see cref="TrackComponentAttribute"/>
		/// and sets them back to null.
		/// </summary>
		void OnSystemDisabled(GameSystem system)
		{
			foreach (var kvp in GetTracking(system)) {
				ContainerRemoveSystem(kvp.Key, system);
				kvp.Value.SetValue(system, null);
			}
		}

		/// <summary>
		/// Returns a collection of properties with a <see cref="TrackComponentAttribute"/>
		/// of the <see cref="GameSystem"/> class, creating it if necessary.
		/// </summary>
		TrackingCollection GetTracking(GameSystem system)
		{
			var container = Game.Systems.GetContainer(system);
			var component = container.GetOrCreate<GameSystemTrackingComponent>();
			return (component.Tracking ?? (component.Tracking = BuildTracking(system)));
		}
		/// <summary>
		/// Builds a collection of properties with a <see cref="TrackComponentAttribute"/>
		/// of the <see cref="GameSystem"/> class and the type of <see cref="GameComponent"/>
		/// it's requesting.
		/// </summary>
		static TrackingCollection BuildTracking(GameSystem system)
		{
			var tracking = new TrackingCollection();
			var pairs = system.GetType().GetMemberAttributes<PropertyInfo, TrackComponentAttribute>(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			
			foreach (var pair in pairs) {
				var type = pair.Attribute.ComponentType;

				if (pair.Member.PropertyType != typeof(IEnumerable<GameEntity>))
					throw pair.MakeException(system.GetType(), "{0}.{1} needs to be type IEnumerable<GameEntity>");
				if (!pair.Member.CanWrite)
					throw pair.MakeException(system.GetType(), "{0}.{1} doesn't have a set accessor");

				PropertyInfo prop;
				if (tracking.TryGetValue(type, out prop))
					throw pair.MakeException(system.GetType(), "{0} is tracking {2} multiple times ({3} and {1})", type.Name, prop.Name);
				
				tracking.Add(type, pair.Member);
			}
			return tracking;
		}

		#endregion

		#region Tracking containers

		/// <summary>
		/// Gets a container for a specific <see cref="GameComponent"/> type.
		/// </summary>
		TrackedComponentContainer GetContainer(Type componentType)
		{
			TrackedComponentContainer container;
			_trackedComponents.TryGetValue(componentType, out container);
			return container;
		}

		/// <summary>
		/// Gets a container for a specific <see cref="GameComponent"/> type,
		/// creating it if necessary.
		/// </summary>
		TrackedComponentContainer GetOrCreateContainer(Type componentType)
		{
			return (GetContainer(componentType) ?? (_trackedComponents[componentType] = new TrackedComponentContainer()));
		}

		/// <summary>
		/// Adds a <see cref="GameSystem"/> to the collection of GameSystems
		/// of a <see cref="TrackedComponentContainer"/>, creating the container
		/// if necessary.
		/// </summary>
		IEnumerable<GameEntity> ContainerAddSystem(Type componentType, GameSystem system)
		{
			var container = GetOrCreateContainer(componentType);
			container.Systems.Add(system);
			return container.EntityEnumerable;
		}

		/// <summary>
		/// Removes a <see cref="GameSystem"/> from the collection of GameSystems
		/// of a <see cref="TrackedComponentContainer"/>, removing the container
		/// from the <see cref="TrackingDictionary"/> if the collection is empty.
		/// </summary>
		void ContainerRemoveSystem(Type componentType, GameSystem system)
		{
			var container = GetContainer(componentType);
			if ((container != null) && container.Systems.Remove(system) && (container.Systems.Count <= 0))
				_trackedComponents.Remove(componentType);
		}


		class TrackingDictionary : Dictionary<Type, TrackedComponentContainer> {  }

		class TrackedComponentContainer
		{
			public ICollection<GameSystem> Systems { get; private set; }
			public ICollection<GameEntity> EntityCollection { get; private set; }
			public IEnumerable<GameEntity> EntityEnumerable { get; private set; }

			public TrackedComponentContainer()
			{
				Systems = new HashSet<GameSystem>();
				EntityCollection = new HashSet<GameEntity>();
				EntityEnumerable = EntityCollection.Select(x => x);
			}
		}

		#endregion
	}
}

