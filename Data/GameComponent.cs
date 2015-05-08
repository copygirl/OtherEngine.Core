using System.Text.RegularExpressions;
using OtherEngine.Core.Events;

namespace OtherEngine.Core.Data
{
	/// <summary>
	/// Represents a small piece of data or state of a <see cref="GameData"/>
	/// or <see cref="GameEntity"/>.
	/// 
	/// Adding a component to a GameEntity will fire a <see cref="GameComponentAddedEvent"/>.
	/// Removing a component from a GameEntity will fire a <see cref="GameComponentRemovedEvent"/>.
	/// </summary>
	public abstract class GameComponent
	{
		static Regex _matchTrailingComponent = new Regex("Component$");

		/// <summary>
		/// Gets the "friendly" name of this GameComponent, without the "Component" suffix.
		/// </summary>
		public string Name { get { return _matchTrailingComponent.Replace(GetType().Name, ""); } }

		public sealed override string ToString()
		{
			return string.Format("[Component: {0}]", Name);
		}
	}
}

