using System;

namespace OtherEngine.Core.Utility
{
	public static class EventExtensions
	{
		public static void Raise(this Action action)
		{
			if (action != null) action();
		}
		public static void Raise<T>(this Action<T> action, T arg)
		{
			if (action != null) action(arg);
		}
		public static void Raise<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			if (action != null) action(arg1, arg2);
		}
		public static void Raise<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			if (action != null) action(arg1, arg2, arg3);
		}
	}
}

