using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Components;
using OtherEngine.Core.Exceptions;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core
{
	/// <summary>
	/// Handles firing of events to GameSystems. Events are automatically
	/// fired to enabled GameSystem methods marked with [SubscribeEvent].
	/// </summary>
	public class CoreEventHandler
	{
		private IDictionary<Type, IDictionary<GameSystem, Action<IGameEvent>>> _events =
			new Dictionary<Type, IDictionary<GameSystem, Action<IGameEvent>>>();

		private readonly Game _game;

		internal CoreEventHandler(Game game)
		{
			_game = game;
		}

		#region GameSystem enabled / disabled

		public void OnSystemEnabled(GameSystem system)
		{
			var container = _game.Systems.GetContainer(system);
			var subscriptions = container.Get<GameSystemEventSubsComponent>();

			if (subscriptions == null) {
				container.Add(subscriptions = new GameSystemEventSubsComponent());
				var systemType = system.GetType();
				foreach (var method in system.GetType().GetAttributes<MethodInfo, SubscribeEventAttribute>()) {
					Type eventType;
					if ((method.GetParameters().Length != 1) ||
						!typeof(IGameEvent).IsAssignableFrom(eventType = method.GetParameters()[0].ParameterType))
						throw new GameSystemException(system, string.Format(
							"[SubscribeEventAttribute]: {0}.{1} must take a single parameter which is an IGameEvent",
							systemType.FullName, method.Name));

					Tuple<MethodInfo, Action<IGameEvent>> checkEvent;
					if (subscriptions.EventListeners.TryGetValue(eventType, out checkEvent))
						throw new GameSystemException(system, string.Format(
							"[SubscribeEventAttribute]: {0} listening for {1} multiple times ({2} and {3})",
							systemType.FullName, eventType, checkEvent.Item1.Name, method.Name));

					var param = Expression.Parameter(typeof(IGameEvent));
					var body = Expression.Call(Expression.Constant(system), method, Expression.Convert(param, eventType));
					var action = Expression.Lambda<Action<IGameEvent>>(body, param).Compile();

					subscriptions.EventListeners.Add(eventType, Tuple.Create(method, action));
				}
			}

			foreach (var entry in subscriptions.EventListeners)
				DoActionOnListeners(entry.Key, (listeners) => listeners.Add(system, entry.Value.Item2));
		}
		public void OnSystemDisabled(GameSystem system)
		{
			var container = _game.Systems.GetContainer(system);
			var subscriptions = container.Get<GameSystemEventSubsComponent>();

			foreach (var entry in subscriptions.EventListeners)
				DoActionOnListeners(entry.Key, (listeners) => listeners.Remove(system));
		}

		/// <summary>
		/// Utility function which creates a copy of the dictionary containing the listeners
		/// for an event type and executes an action on it (such as adding or removing an entry).
		/// This is to prevent a concurrent modification exception when firing events.
		/// </summary>
		private void DoActionOnListeners(Type eventType, Action<Dictionary<GameSystem, Action<IGameEvent>>> action)
		{
			IDictionary<GameSystem, Action<IGameEvent>> eventList;
			_events.TryGetValue(eventType, out eventList);
			var newEventList = ((eventList != null) 
				? new Dictionary<GameSystem, Action<IGameEvent>>(eventList)
				: new Dictionary<GameSystem, Action<IGameEvent>>());
			
			action(newEventList);

			if (newEventList.Count > 0)
				_events[eventType] = newEventList;
			else _events.Remove(eventType);
		}

		#endregion

		#region Firing events

		// TODO: Add attribute for base classes which are not supposed to be fired as events.
		//       For example: GameSystemEvent and GameComponentEvent.

		// FIXME: Events currently not being fired currently.
		//        Some fire multiple times, and they're out of order.

		/// <summary>
		/// Fires an event to all GameSystems that are listening to it.
		/// Example fire order:
		/// - GameComponentAddedEvent<TestComponent>
		/// - GameComponentAddedEvent<IGameComponent>
		/// - GameComponentEvent<TestComponent>
		/// - GameComponentEvent<IGameComponent>
		/// - IGameEvent
		/// </summary>
		public void Fire<TEvent>(params object[] arguments) where TEvent : IGameEvent
		{
			Fire(typeof(TEvent), arguments);
		}
		public void Fire(Type eventType, Type genericType, params object[] arguments)
		{
			if (eventType == null)
				throw new ArgumentNullException("eventType");
			if (genericType == null)
				throw new ArgumentNullException("genericType");

			eventType = eventType.MakeGenericType(genericType);
			if (typeof(IGameEvent).IsAssignableFrom(eventType))
				throw new ArgumentException(string.Format(
					"{0} is not an IGameEvent", eventType), "eventType");
			
			Fire(eventType, arguments);
		}

		private void Fire(Type eventType, params object[] arguments)
		{
			if (eventType.IsGenericType)
				FireGeneric(eventType, arguments);
			FireNonGeneric(eventType, (IGameEvent)Activator.CreateInstance(eventType, arguments));
		}
		private void FireNonGeneric(Type eventType, IGameEvent gameEvent)
		{
			if (typeof(IGameEvent).IsAssignableFrom(eventType.BaseType))
				FireNonGeneric(eventType.BaseType, gameEvent);
			else foreach (var i in eventType.GetInterfaces())
				FireNonGeneric(i, gameEvent);
			
			FireSingle(eventType, gameEvent);
		}

		private void FireGeneric(Type eventType, object[] arguments)
		{
			// TODO: Support multiple generic type arguments?
			var def = eventType.GetGenericTypeDefinition();
			var arg = eventType.GetGenericArguments()[0];
			var constraint = def.GetGenericArguments()[0].GetGenericParameterConstraints()[0];
			if (arg == constraint) return;
			FireGeneric(def, arg, constraint, arguments);
		}
		private void FireGeneric(Type eventTypeDef, Type arg, Type constraint, object[] arguments)
		{
			if (constraint.IsAssignableFrom(arg.BaseType) &&
				!arg.BaseType.IsGenericTypeDefinition) {
				arg = arg.BaseType;
				FireGeneric(eventTypeDef, arg, constraint, arguments);
			} else if (arg.GetInterfaces().Contains(constraint)) {
				// TODO: Support interface inheritance.
				arg = constraint;
			} else return;
			arg = eventTypeDef.MakeGenericType(arg);
			FireSingle(arg, (IGameEvent)Activator.CreateInstance(arg, arguments));
		}

		/// <summary>
		/// Fires a single event which only GameSystems
		/// listening to this exact eventType will receive.
		/// </summary>
		private void FireSingle(Type eventType, IGameEvent gameEvent)
		{
			Console.WriteLine(eventType);
			IDictionary<GameSystem, Action<IGameEvent>> listeners;
			if (!_events.TryGetValue(eventType, out listeners)) return;
			foreach (var listener in listeners) {
				// If an exception occurs when sending the event to a game system,
				// handle this exception without cancelling the event or causing
				// other systems to fail.
				try { listener.Value(gameEvent); }
				catch (GameSystemException ex) { _game.Systems.OnException(ex.System, ex); }
				catch (Exception ex) { _game.Systems.OnException(listener.Key, ex); }
			}
		}

		#endregion
	}
}

