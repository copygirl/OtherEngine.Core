using System;
using System.Linq;
using OtherEngine.Core.Data;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Systems;
using OtherEngine.Core.Events;
using OtherEngine.Core.Components;
using OtherEngine.Core.Attributes;

namespace OtherEngine.Core
{
	/// <summary>
	/// Handles modules, which are any assemblies that contain <see cref="GameComponent"/>,
	/// <see cref="GameSystem"/> and/or <see cref="IGameEvent"/> types. When enumerated,
	/// returns all <see cref="GameData"/> objects which will contain data related to
	/// these modules.
	/// </summary>
	public class CoreModuleHandler : IEnumerable<GameData>
	{
		readonly IDictionary<Assembly, GameData> _moduleContainers = new Dictionary<Assembly, GameData>();

		readonly Game _game;

		internal CoreModuleHandler(Game game)
		{
			_game = game;
		}

		/// <summary>
		/// Loads a module directly from an assembly, auto enabling all
		/// <see cref="GameSystem"/>s with <see cref="AutoEnableAttribute"/>s.
		/// </summary>
		public void LoadModule(Assembly assembly)
		{
			var container = GetContainer(assembly);
			if (container == null)
				throw new ArgumentException("{0} is not a module");
			var component = container.GetOrThrow<GameModuleContainerComponent>();
			if (component.Loaded) return;

			foreach (var systemType in component.SystemTypes)
				if (_game.Systems.GetContainer(systemType).GetOrThrow<GameSystemContainerComponent>().AutoEnable) {
					var method = typeof(CoreSystemHandler).GetMethod("Get").MakeGenericMethod(systemType);
					((GameSystem)method.Invoke(_game.Systems, new object[0])).Enable();
				}

			component.Loaded = true;
		}

		/// <summary>
		/// Returns a module container for this assembly, creating it if
		/// necessary. Returns null for assemblies which aren't modules.
		/// </summary>
		public GameData GetContainer(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			GameData container;
			return (_moduleContainers.TryGetValue(assembly, out container) ? container
				: (_moduleContainers[assembly] = BuildContainer(assembly)));
		}

		/// <summary>
		/// Builds the module container for this assembly, collecting its <see cref="GameComponent"/>,
		/// <see cref="GameSystem"/> and <see cref="IGameEvent"/> types. Returns null if there's none
		/// of either of these types.
		/// </summary>
		GameData BuildContainer(Assembly assembly)
		{
			var components = new List<Type>();
			var systems = new List<Type>();
			var events = new List<Type>();

			foreach (var type in assembly.GetTypes()) {
				if (type.IsInterface || type.IsAbstract)
					continue;
				BuildHelper<GameComponent>(type, components);
				BuildHelper<GameSystem>(type, systems, SetupSystem);
				BuildHelper<IGameEvent>(type, events);
			}

			if (components.Count + systems.Count + events.Count <= 0)
				return null;

			var container = new GameModuleContainerComponent(assembly,
				components.Select(x => x), systems.Select(x => x), events.Select(x => x));
			return new GameData { container };
		}
		static void BuildHelper<TType>(Type type, ICollection<Type> list, Action<Type> action = null)
		{
			if (!typeof(TType).IsAssignableFrom(type)) return;
			list.Add(type);
			if (action != null)
				action(type);
		}

		void SetupSystem(Type systemType)
		{
			if (systemType.GetCustomAttribute<AutoEnableAttribute>() != null)
				_game.Systems.GetContainer(systemType).GetOrThrow<GameSystemContainerComponent>().AutoEnable = true;
		}

		#region IEnumerable implementation

		public IEnumerator<GameData> GetEnumerator()
		{
			return _moduleContainers.Values.Where(x => (x != null)).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

