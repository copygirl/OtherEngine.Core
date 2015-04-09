using System;
using OtherEngine.Core.Events;
using System.Reflection;

namespace OtherEngine.Core.Utility
{
	public static class TypeUtils
	{
		public static string ToString(Type type)
		{
			return type.ToString();
		}
		public static string ToString(object obj)
		{
			return ToString(obj.GetType());
		}
		public static string ToString<T>()
		{
			return ToString(typeof(T));
		}
	}
}

