using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Systems
{
	public static class SystemExtensions
	{
		public static void CheckedForeach(this IEnumerable<GameSystem> systems, Action<GameSystem> action)
		{
			foreach (var system in systems) {
				try { action(system); }
				catch (Exception ex) { system.Game.Systems.OnError(system, ex); }
			}
		}
	}
}

