using System;
using System.Text.RegularExpressions;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core
{
	/// <summary>
	/// Contains a small, atomic piece of information which can
	/// be attached to an entity. Components are looked up by their
	/// type, so there can only be one of each type per entity.
	/// </summary>
	public abstract class Component
	{
		/// <summary>
		/// Returns the name of the component, without the "Component" suffix.
		/// </summary>
		public string Name { get { return GetName(GetType()); } }


		#region GetName / ToString related

		static readonly Regex _removeComponentRegex = new Regex("Component(?:<.*>)?$");

		/// <summary>
		/// Returns the simple name of the component type, without the "Component" suffix.
		/// </summary>
		public static string GetName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsSubclassOf(typeof(Component)))
				throw new ArgumentException(string.Format(
					"{0} is not a Component", type), "type");

			return _removeComponentRegex.Replace(type.GetPrettyName(), "");
		}

		public static string ToString(Type type)
		{
			return string.Format("[Component {0}]", GetName(type));
		}

		public sealed override string ToString()
		{
			return ToString(GetType());
		}

		#endregion
	}
}

