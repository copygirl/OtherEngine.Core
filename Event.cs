using System;
using System.Text.RegularExpressions;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core
{
	/// <summary>
	/// Represents an event that controllers can subscribe to in various ways.
	/// </summary>
	public abstract class Event
	{
		/// <summary>
		/// Gets the name of the event, without the "Event" suffix.
		/// </summary>
		public string Name { get { return GetName(GetType()); } }


		#region GetName / ToString related

		static readonly Regex _removeEventRegex = new Regex("Event(?:<.*>)?$");

		/// <summary>
		/// Returns the simple name of the event type, without the "Event" suffix.
		/// </summary>
		public static string GetName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsSubclassOf(typeof(Event)))
				throw new ArgumentException(string.Format(
					"{0} is not an Event", type), "type");

			return _removeEventRegex.Replace(type.GetPrettyName(), "");
		}

		public static string ToString(Type type)
		{
			return string.Format("[Event {0}]", GetName(type));
		}

		public override string ToString()
		{
			return ToString(GetType());
		}

		#endregion
	}
}

