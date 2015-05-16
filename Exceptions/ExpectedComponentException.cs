using System;
using OtherEngine.Core.Data;

namespace OtherEngine.Core.Exceptions
{
	/// <summary>
	/// Thrown when a certain <see cref="GameComponent"/> was expected on a
	/// <see cref="GameData"/> or <see cref="GameEntity"/>, but wasn't found.
	/// </summary>
	public abstract class ExpectedComponentException : Exception
	{
		public GameData Data { get; private set; }
		public Type ComponentType { get; private set; }

		internal ExpectedComponentException(Type componentType, GameData data, string message = null)
			: base(message ?? GetDefaultMessage(componentType, data))
		{
			Data = data;
			ComponentType = componentType;
		}

		static string GetDefaultMessage(Type componentType, GameData data)
		{
			return string.Format("{0} was expected on {1}", GameComponent.ToString(componentType), data);
		}
	}

	/// <summary>
	/// Thrown when a certain <see cref="GameComponent"/> was expected on a
	/// <see cref="GameData"/> or <see cref="GameEntity"/>, but wasn't found.
	/// </summary>
	public class ExpectedComponentException<TComponent> : ExpectedComponentException where TComponent : GameComponent
	{
		public ExpectedComponentException(GameData data, string message = null)
			: base(typeof(TComponent), data, message) {  }
	}
}

