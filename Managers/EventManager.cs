using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Components;
using OtherEngine.Core.Events;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Managers
{
	/// <summary>
	/// Handles firing of events and event containers.
	/// </summary>
	public class EventManager : ContainerManager<Event>
	{
		LinkedList<Event> _delayedEvents;
		LinkedListNode<Event> _currentEvent;


		internal EventManager(Game game) : base(game) {  }


		#region Controller enabling / disabling

		/// <summary>
		/// Called when a controller is enabled.
		/// If the controller was enabled for the first time, builds its event subscription list.
		/// Adds the subscriptions to the event dictionary and fires a ControllerEnabledEvent.
		/// </summary>
		internal void OnControllerEnabled(Controller controller)
		{
			var container = Game.Controllers.GetContainer(controller);

			var component = container.Get<ControllerSubscriptionsComponent>();
			if (component == null)
				container.Add(component = BuildEventSubscriptions(controller));

			foreach (var sub in component.Value) {
				var eventContainer = GetContainer(sub.EventType);
				var eventSubs = eventContainer.GetOrThrow<EventSubscriptionComponent>();
				eventSubs.Value += sub.Action;
			}

			Fire(new ControllerEnabledEvent(controller));
		}

		/// <summary>
		/// Called when a controller is disabled.
		/// Removes the subscriptions from the event dictionary and fires a ControllerDisabledEvent.
		/// </summary>
		internal void OnControllerDisabled(Controller controller)
		{
			var container = Game.Controllers.GetContainer(controller);
			var component = container.GetOrThrow<ControllerSubscriptionsComponent>();

			foreach (var sub in component.Value) {
				var eventContainer = GetContainer(sub.EventType);
				var eventSubs = eventContainer.GetOrThrow<EventSubscriptionComponent>();
				eventSubs.Value -= sub.Action;
			}
			
			Fire(new ControllerDisabledEvent(controller));
		}

		#endregion

		#region Building controller event subscriptions

		/// <summary>
		/// Collects all members (methods and properties) of the controller and
		/// builds an Action&lt;Event&gt; for each to call when an event is fired.
		/// </summary>
		ControllerSubscriptionsComponent BuildEventSubscriptions(Controller controller)
		{
			var component = new ControllerSubscriptionsComponent();

			// Get and iterate all members with the SubscribeEvent attribute.
			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var members = controller.GetType().GetMembersWithAttribute<MemberInfo, SubscribeEventAttribute>(flags);

			foreach (var pair in members) {
				Type eventType;
				Action<Event> action;

				if (pair.Member is MethodInfo) {

					var method = (MethodInfo)pair.Member;
					eventType = method.GetParameters()[0].ParameterType;

					// Create an Action<Event> that converts the event parameter to
					// eventType and calls the method (an Action<[eventType]>).
					var parameter = Expression.Parameter(typeof(Event));
					var body = Expression.Call(Expression.Constant(controller), method, Expression.Convert(parameter, eventType));
					action = Expression.Lambda<Action<Event>>(body, parameter).Compile();

					// TODO: Implement error checking code.

				} else {

					var property = (PropertyInfo)pair.Member;
					eventType = property.PropertyType.GetGenericArguments()[0];

					// Create an EventQueue<[eventType]> and an action which calls its Push method.
					var queue = Activator.CreateInstance(typeof(EventQueue<>).MakeGenericType(eventType));
					action = (Action<Event>)Delegate.CreateDelegate(typeof(Action<Event>), queue, "Push");

				}

				component.Value.Add(new EventSubscription(eventType, action));
			}

			return component;
		}

		#endregion


		/// <summary>
		/// Fires the event, which can be received by
		/// controllers using the SubscribeEvent attribute.
		/// </summary>
		public void Fire(Event ev)
		{
			if (ev == null)
				throw new ArgumentNullException("ev");

			if (_delayedEvents != null) {
				if (_currentEvent != null)
					_currentEvent = _delayedEvents.AddAfter(_currentEvent, ev);
				else _delayedEvents.AddLast(ev);
				return;
			}

			FireInternal(ev);
		}
		internal void FireInternal(Event ev)
		{
			var eventContainer = GetContainer(ev.GetType());
			var eventSubs = eventContainer.GetOrThrow<EventSubscriptionComponent>();

			eventSubs.Value?.Invoke(ev);
		}


		/// <summary>
		/// Starts delaying events until FireDelayedEvents is called.
		/// </summary>
		internal void DelayEvents()
		{
			_delayedEvents = new LinkedList<Event>();
		}

		/// <summary>
		/// Fires all events that were delayed since DelayEvents was called.
		/// 
		/// If more events are fired because of the delayed events, they're
		/// queued up to be fired after the initial event. This way, all events
		/// should be fired in the same order as if they weren't delayed.
		/// </summary>
		internal void FireDelayedEvents()
		{
			for (var ev = _currentEvent = _delayedEvents.First; ev != null; ev = _currentEvent = ev.Next)
				FireInternal(ev.Value);
			_delayedEvents = null;
		}


		protected override Entity CreateContainer(Type type)
		{
			var container = base.CreateContainer(type);
			container.Add(new NameComponent{ Value = Event.GetName(type) });
			container.Add(new EventSubscriptionComponent());
			return container;
		}


		#region Helper classes

		/// <summary>
		/// Component added to event containers to hold currently subscribed actions.
		/// </summary>
		class EventSubscriptionComponent : SimplePublicComponent<Action<Event>> {  }

		/// <summary>
		/// Component added to controller container entities to hold event subscription data.
		/// </summary>
		class ControllerSubscriptionsComponent : SimpleComponent<List<EventSubscription>>
		{
			public ControllerSubscriptionsComponent() { Value = new List<EventSubscription>(); }
		}

		/// <summary>
		/// Represents a single event subscription of a controller.
		/// </summary>
		class EventSubscription
		{
			public Type EventType { get; private set; }
			public Action<Event> Action { get; private set; }

			public EventSubscription(Type eventType, Action<Event> action)
			{
				EventType = eventType;
				Action = action;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		class EventQueue<TEvent> : IEnumerable<TEvent>
			where TEvent : Event
		{
			readonly Queue<TEvent> _queue = new Queue<TEvent>();

			public void Push(Event ev)
			{
				lock (_queue)
					_queue.Enqueue((TEvent)ev);
			}

			#region IEnumerable implementation

			public IEnumerator<TEvent> GetEnumerator()
			{
				while (_queue.Count > 0)
					lock (_queue)
						yield return _queue.Dequeue();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}

		#endregion
	}
}

