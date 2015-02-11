using System;
using System.Collections.Generic;
using OtherEngine.Core.Utility;
using OtherEngine.Core.Exceptions;
using System.Reflection;
using OtherEngine.Core.Collections;

namespace OtherEngine.Core.Systems
{
	public class GameSystemCollection : IEnumerable<GameSystem>
	{
		private readonly SystemCollection _systems;

		private readonly Game _game;

		public GameSystemCollection(Game game)
		{
			_game = game;
			_systems = new SystemCollection(game);
		}

		public T Get<T>() where T : GameSystem
		{
			return _systems.Get<T>(TypedGetBehavior.Default);
		}

		public void Enable<T>() where T : GameSystem
		{
			var system = _systems.Get<T>(TypedGetBehavior.CreateAndAdd);
			if (system.State >= GameSystemState.Enabled) return;

			try {
				system.OnEnabled();
				system.State = GameSystemState.Enabled;
			} catch (Exception ex) {
				system.State = GameSystemState.Errored;
				throw new GameSystemException(system,
					String.Format("Exception when enabling '{0}': {1}", this, ex.Message), ex);
			}
		}
		public void Disable<T>() where T : GameSystem
		{
			var system = Get<T>();
			if ((system == null) || (system.State <= GameSystemState.Disabled)) return;

			try {
				system.OnDisabled();
				system.State = GameSystemState.Disabled;
			} catch (Exception ex) {
				system.State = GameSystemState.Errored;
				throw new GameSystemException(system,
					String.Format("Exception when disabling '{0}': {1}", this, ex.Message), ex);
			}
		}

		#region IEnumerable implementation

		public IEnumerator<GameSystem> GetEnumerator()
		{
			return _systems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private class SystemCollection : TypedDefaultCollection<GameSystem>
		{
			private readonly Game _game;

			public SystemCollection(Game game)
			{
				_game = game;
			}

			protected override GameSystem NewValue<TSystem>()
			{
				try {
					var system = Activator.CreateInstance<TSystem>();
					system.Game = _game;
					return system;
				} catch (MissingMethodException ex) {
					throw new GameSystemException(typeof(TSystem),
						String.Format("'{0}' does not have a parameterless constructor", typeof(TSystem)), ex);
				} catch (TargetInvocationException ex) {
					throw new GameSystemException(typeof(TSystem),
						String.Format("Exception in constructor of '{0}': {1}", typeof(TSystem), ex.Message), ex);
				} catch (Exception ex) {
					throw new GameSystemException(typeof(TSystem),
						String.Format("Exception when constructing '{0}': {1}", typeof(TSystem), ex.Message), ex);
				}
			}
		}
	}
}

