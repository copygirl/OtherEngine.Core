using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Components;
using OtherEngine.Core.Events;
using OtherEngine.Core.Systems;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core
{
	/// <summary>
	/// Handles firing of <see cref="IGameEvent"/>s to <see cref="GameSystem"/>s.
	/// Events are automatically fired to enabled GameSystem methods marked with 
	/// the <see cref="SubscribeEventAttribute"/>.
	/// </summary>
	public class CoreEventHandler
	{
		readonly EventDictionary _events = new EventDictionary();
		readonly Game _game;

		internal CoreEventHandler(Game game)
		{
			_game = game;
		}

		#region GameSystem enabled / disabled

		internal void OnSystemEnabled(GameSystem system)
		{
			foreach (var entry in GetSubscriptions(system))
				DoActionOnListeners(entry.Key, listeners => listeners.Add(system, entry.Value));
		}
		internal void OnSystemDisabled(GameSystem system)
		{
			foreach (var entry in GetSubscriptions(system))
				DoActionOnListeners(entry.Key, listeners => listeners.Remove(system));
		}

		SubscriptionCollection GetSubscriptions(GameSystem system)
		{
			var container = _game.Systems.GetContainer(system);
			var component = container.GetOrCreate<GameSystemSubscriptionComponent>();
			return (component.Subscriptions ?? (component.Subscriptions = BuildSubscriptions(system)));
		}

		#region Building event subscription list

		static SubscriptionCollection BuildSubscriptions(GameSystem system)
		{
			var subscriptions = new SubscriptionCollection();
			var pairs = system.GetType().GetMemberAttributes<MethodInfo, SubscribeEventAttribute>(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			
			foreach (var pair in pairs) {
				if (pair.Member.IsStatic)
					throw pair.MakeException(system.GetType(), "{0}.{1} can't be static");
				if (pair.Member.IsAbstract)
					throw pair.MakeException(system.GetType(), "{0}.{1} can't be abstract");
				if (!pair.Member.IsPublic)
					throw pair.MakeException(system.GetType(), "{0}.{1} needs to be public");
				
				var parameters = pair.Member.GetParameters();
				Type eventType = null;
				if ((parameters.Length != 1) || !typeof(IGameEvent).IsAssignableFrom(eventType = parameters[0].ParameterType))
					throw pair.MakeException(system.GetType(), "{0}.{1} needs exactly one parameter of type IGameEvent");
				if ((eventType != typeof(IGameEvent)) && (eventType.IsInterface || eventType.IsAbstract))
					throw pair.MakeException(system.GetType(), "{0}.{1} parameter is not a concrete type (not interface or abstract) or IGameEvent");

				EventSubscription sub;
				if (subscriptions.TryGetValue(eventType, out sub))
					throw pair.MakeException(system.GetType(), "{0} is subscribing to {2} multiple times ({3} and {1})", eventType.Name, sub.Method.Name);

				Action<IGameEvent> action;
				try { action = BuildAction(system, pair.Member, eventType); }
				catch (Exception ex) { throw pair.MakeException(system.GetType(), ex); }

				subscriptions.Add(eventType, new EventSubscription(pair.Member, action));
			}
			return subscriptions;
		}

		/// <summary>
		/// Builds and compiles the action (which takes an IGameEvent) which will eventually be used
		/// to call the <see cref="GameSystem"/>'s subscription method (which takes a specific IGameEvent
		/// type, meaning it needs to be dynamically cast).
		/// </summary>
		static Action<IGameEvent> BuildAction(GameSystem system, MethodInfo method, Type eventType)
		{
			var param = Expression.Parameter(typeof(IGameEvent));
			var body = Expression.Call(Expression.Constant(system), method, Expression.Convert(param, eventType));
			return Expression.Lambda<Action<IGameEvent>>(body, param).Compile();
		}

		#endregion

		/// <summary>
		/// Utility function which creates a copy of the dictionary containing the listeners
		/// for an event type and executes an action on it (such as adding or removing an entry).
		/// This is to prevent a concurrent modification exception when firing events.
		/// </summary>
		void DoActionOnListeners(Type eventType, Action<EventCollection> action)
		{
			EventCollection eventList;
			_events.TryGetValue(eventType, out eventList);
			var newEventList = ((eventList != null) 
				? new EventCollection(eventList)
				: new EventCollection());
			
			action(newEventList);

			if (newEventList.Count > 0)
				_events[eventType] = newEventList;
			else _events.Remove(eventType);
		}

		#endregion

		#region Firing events

		public void Fire(IGameEvent gameEvent)
		{
			if (gameEvent == null)
				throw new ArgumentNullException("gameEvent");
			
			Fire(gameEvent.GetType(), gameEvent);
			Fire(typeof(IGameEvent), gameEvent);
		}
		void Fire(Type eventType, IGameEvent gameEvent)
		{
			EventCollection listeners;
			if (!_events.TryGetValue(eventType, out listeners)) return;
			foreach (var listener in listeners)
				// TODO: Handle exceptions.
				listener.Value.Call(gameEvent);
		}

		#endregion

		class EventDictionary : Dictionary<Type, EventCollection> {  }

		class EventCollection : Dictionary<GameSystem, EventSubscription>
		{
			public EventCollection() : base() {  }
			public EventCollection(EventCollection collection) : base(collection) {  }
		}
	}
}

