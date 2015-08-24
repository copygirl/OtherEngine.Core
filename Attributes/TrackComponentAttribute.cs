using System;
using System.Reflection;
using System.Collections.Generic;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// A controller property with this attribute will be set to a
	/// readonly collection of entities with the specified component type.
	/// 
	/// When controllers are loaded late, entities that had this type
	/// of component added before may not be included in the collection.
	/// </summary>
	/// <example>
	/// 	[TrackComponent(typeof(ExampleComponent))]
	/// 	IReadOnlyCollection<Entity> ExampleEntities { get; set; }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class TrackComponentAttribute : ValidatedAttribute
	{
		public Type ComponentType { get; private set; }

		public TrackComponentAttribute(Type componentType)
		{
			ComponentType = componentType;
		}

		public override void Validate(ICustomAttributeProvider target)
		{
			var property = (PropertyInfo)target;
			if (!property.PropertyType.Is(typeof(IReadOnlyCollection<>)))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not an IReadOnlyCollection<>", target.GetName()));
		}
	}
}

