using System;
using System.Text.RegularExpressions;
using OtherEngine.Core.Events;

namespace OtherEngine.Core.Data
{
	/// <summary>
	/// Represents a small piece of data or state of a <see cref="GameData"/>
	/// or <see cref="GameEntity"/>.
	/// 
	/// Adding a component to a GameEntity will fire a
	/// <see cref="GameComponentAddedEvent&lt;TComponent&gt;"/>.
	/// 
	/// Removing a component from a GameEntity will fire a
	/// <see cref="GameComponentRemovedEvent&lt;TComponent&gt;"/>.
	/// </summary>
	public abstract class GameComponent
	{
		static Regex _matchTrailingComponent = new Regex("Component$");

		/// <summary>
		/// Gets the "friendly" name of this GameComponent, without the "Component" suffix.
		/// </summary>
		public string Name { get { return GetName(GetType()); } }

		#region ToString / name related

		static string ToString(string name)
		{
			return string.Format("[Component: {0}]", name);
		}

		public sealed override string ToString()
		{
			return ToString(Name);
		}

		/// <summary>
		/// Returns the name of a GameComponent type, same as the instance version.
		/// </summary>
		public static string GetName(Type componentType)
		{
			if (componentType == null)
				throw new ArgumentNullException("componentType");
			if (!componentType.IsSubclassOf(typeof(GameComponent)))
				throw new ArgumentException(String.Format(
					"{0} is not a GameComponent", componentType), "componentType");

			return _matchTrailingComponent.Replace(componentType.Name, "");
		}

		/// <summary>
		/// Returns a string representation of the GameComponent type, same as the instance version.
		/// </summary>
		public static string ToString(Type componentType)
		{
			return ToString(GetName(componentType));
		}

		#endregion
	}
}

