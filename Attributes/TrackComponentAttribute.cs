using System;
using OtherEngine.Core.Data;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// Controlled by <see cref="ComponentTrackerSystem"/>.
	/// 
	/// A <see cref="GameSystem"/> property with this attribute will be set
	/// to a enumerable of <see cref="GameEntity"/>s when the GameSystem is
	/// enabled. If either system is disabled, the property will be reset
	/// to null;
	/// </summary>
	/// <example>
	/// 	[TrackComponent(typeof(SomeComponent))]
	/// 	private IEnumerable<GameEntity> SomeEntities { get; set; }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class TrackComponentAttribute : Attribute
	{
		public Type ComponentType { get; private set; }

		public TrackComponentAttribute(Type componentType)
		{
			ComponentType = componentType;
		}
	}
}

