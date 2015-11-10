using System;
using System.Collections.Generic;
using System.Reflection;

namespace OtherEngine.Core
{
	public class ModuleController : IReadOnlyCollection<ModuleContainer>
	{
		readonly Dictionary<Assembly, ModuleContainer> _modules =
			new Dictionary<Assembly, ModuleContainer>();

		readonly Game _game;


		internal ModuleController(Game game)
		{
			_game = game;
		}


		/// <summary> Loads a module with all its engine types (components and processors)
		///           from the specified assembly. This activates any auto-enable processors. </summary>
		public ModuleContainer Load(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			if (_game.State != GameState.Initializing)
				throw new InvalidOperationException(
					"Can't load modules while game isn't initializing");
			if (_modules.ContainsKey(assembly))
				throw new InvalidOperationException(string.Format(
					"Attempted to load module {0} multiple times", assembly));

			var module = new ModuleContainer(_game, assembly);
			_modules.Add(assembly, module);

			_game.Processors.OnModuleLoaded(module);
			_game.Threads.OnModuleLoaded(module);

			return module;
		}


		#region IReadOnlyCollection<ModuleContainer> implementation

		public int Count { get { return _modules.Count; } }

		public IEnumerator<ModuleContainer> GetEnumerator()
		{
			return _modules.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

