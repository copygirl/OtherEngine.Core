using System;
using System.Collections.Generic;
using System.Linq;

namespace OtherEngine.Core.Systems
{
	public class CoreEventRegistry
	{
		private readonly Game _game;

		private readonly ICollection<Type> _autoInterfaces = new HashSet<Type>();
		private readonly IDictionary<Type, ICollection<GameSystem>> _watchedInterfaces = new Dictionary<Type, ICollection<GameSystem>>();
		private readonly IDictionary<Type, IEnumerable<Type>> _systemCache = new Dictionary<Type, IEnumerable<Type>>();

		internal CoreEventRegistry(Game game)
		{
			_game = game;
		}

		public void AutoRegister(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsInterface)
				throw new ArgumentException(String.Format("{0} is not an interface type.", type));
			_autoInterfaces.Add(type);
		}

		public IEnumerable<GameSystem> GetSystems(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			ICollection<GameSystem> systems;
			return (_watchedInterfaces.TryGetValue(type, out systems) ? systems : Enumerable.Empty<GameSystem>());
		}
		public IEnumerable<GameSystem> GetSystems<T>()
		{
			return GetSystems(typeof(T));
		}

		internal void OnSystemEnabled(GameSystem system)
		{
			var type = system.GetType();
			IEnumerable<Type> interfaces;
			if (!_systemCache.TryGetValue(type, out interfaces))
				_systemCache[type] = interfaces = type.GetInterfaces().Where(
					t => _autoInterfaces.Contains(t.IsGenericType ? t.GetGenericTypeDefinition() : t)).ToArray();

			foreach (var i in interfaces) {
				ICollection<GameSystem> systems;
				if (!_watchedInterfaces.TryGetValue(i, out systems))
					_watchedInterfaces[i] = systems = new HashSet<GameSystem>();

				systems.Add(system);
			}
		}

		internal void OnSystemDisabled(GameSystem system)
		{
			var type = system.GetType();
			IEnumerable<Type> interfaces;
			if (_systemCache.TryGetValue(type, out interfaces))
				foreach (var i in _systemCache[type])
					_watchedInterfaces[i].Remove(system);
		}
	}
}

