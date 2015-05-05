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
		private readonly IDictionary<Type, IDictionary<GameSystem, Action<IGameEvent>>> _events =
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
		// TODO: Allow events with multiple generic parameters.

		public void Fire<TEvent>(params object[] arguments) where TEvent : IGameEvent
		{
			Fire(typeof(TEvent), arguments);
		}
		public void Fire(Type eventType, params object[] arguments)
		{
			if (eventType == null)
				throw new ArgumentNullException("eventType");
			if (arguments == null)
				throw new ArgumentNullException("arguments");

			var gameEvent = (IGameEvent)Activator.CreateInstance(eventType, arguments);
			FireRecursive(eventType, gameEvent);
			FireSingle(typeof(IGameEvent), gameEvent);
		}

		public void Fire(Type eventTypeDef, Type[] typeArgs, params object[] arguments)
		{
			// TODO: Caching this would probably make sense.

			if (eventTypeDef == null)
				throw new ArgumentNullException("eventTypeDef");
			if (typeArgs == null)
				throw new ArgumentNullException("typeArgs");
			
			Type eventType;
			try { eventType = eventTypeDef.MakeGenericType(typeArgs); }
			catch (Exception ex) {
				throw new ArgumentException(String.Format(
					"Could not create generic type of {0} using type arguments [{1}]: {2}",
					eventType, string.Join(",", typeArgs), ex.Message), ex);
			}

			IGameEvent gameEvent;
			gameEvent = (IGameEvent)Activator.CreateInstance(eventType, arguments);

			FireRecursive(eventType, gameEvent, eventTypeDef);
			FireSingle(typeof(IGameEvent), gameEvent);
		}
		public void Fire(Type eventTypeDef, Type typeArg, params object[] arguments)
		{
			Fire(eventTypeDef, new { typeArg }, arguments);
		}

		private void FireRecursive(Type eventType, IGameEvent gameEvent, Type eventTypeDef = null)
		{
			FireSingle(eventType, gameEvent);

			if (eventType.IsGenericType)

			if (typeof(IGameEvent).IsAssignableFrom(eventType.BaseType))
				FireRecursive(eventType.BaseType, gameEvent);
			// TODO: What about interfaces?
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

