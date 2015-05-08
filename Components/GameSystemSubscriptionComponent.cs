using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Data;
using OtherEngine.Core.Events;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Component of a <see cref="GameSystem"/> container which
	/// contains the <see cref="IGameEvent"/>s the GameSystem is listening to.
	/// </summary>
	internal class GameSystemSubscriptionComponent : GameComponent
	{
		public SubscriptionCollection Subscriptions { get; set; }
	}

	/// <summary>
	/// Represents the list of <see cref="IGameEvent"/>s a
	/// <see cref="GameSystem"/> is listening to.
	/// </summary>
	internal class SubscriptionCollection : Dictionary<Type, EventSubscription> {  }

	/// <summary>
	/// Represents a single <see cref="IGameEvent"/> a <see cref="GameSystem"/> is listening to.
	/// </summary>
	internal class EventSubscription
	{
		readonly Action<IGameEvent> _action;
		public MethodInfo Method { get; private set; }

		public EventSubscription(MethodInfo method, Action<IGameEvent> action)
		{
			Method = method;
			_action = action;
		}

		public void Call(IGameEvent ev) { _action(ev); }
	}
}

