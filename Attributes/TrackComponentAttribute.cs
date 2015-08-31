using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			if (componentType == null)
				throw new ArgumentNullException("componentType");
			if (!componentType.IsSubclassOf(typeof(Component)))
				throw new ArgumentException(string.Format(
					"{0} is not a Component", componentType), "componentType");
			if (componentType.IsAbstract)
				throw new ArgumentException(string.Format(
					"{0} is abstract", componentType), "componentType");
			
			ComponentType = componentType;
		}


		public override void Validate(ICustomAttributeProvider target)
		{
			var property = (PropertyInfo)target;
			if (property.PropertyType != typeof(IReadOnlyCollection<Entity>))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not an IReadOnlyCollection<Entity>", target.GetName()));
		}

		public override void Validate(IReadOnlyCollection<MemberValidatedAttributePair> pairs)
		{
			if (pairs.Count <= 1) return;

			var pairsByComponentType = pairs.GroupBy(pair =>
				((TrackComponentAttribute)pair.Attribute).ComponentType);

			foreach (var componentTypePairs in pairsByComponentType)
				if (componentTypePairs.Count() > 1)
					throw new AttributeUsageException(this, null, string.Format(
						"{0} tracked multiple times", componentTypePairs.Key.Name));
		}
	}
}

