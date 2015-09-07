using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Tracking
{
	/// <summary>
	/// A controller property with this attribute will be set to a
	/// readonly collection of entities with the specified component type.
	/// 
	/// When controllers are loaded late, entities that had this type
	/// of component added before may not be included in the collection.
	/// </summary>
	/// <example>
	/// 	[TrackComponent]
	/// 	EntityCollection<ExampleComponent> Examples { get; private set; }
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class TrackComponentAttribute : ValidatedAttribute
	{
		public override void Validate(ICustomAttributeProvider target)
		{
			if (!((MemberInfo)target).DeclaringType.IsSubclassOf(typeof(Controller)))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not declared on a Controller", target.GetName()));
			
			var property = (PropertyInfo)target;
			if (!property.PropertyType.Is(typeof(EntityCollection<>)))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not an EntityCollection<>", target.GetName()));
			
			var componentType = property.PropertyType.GetGenericArguments()[0];
			if (!componentType.IsSubclassOf(typeof(Component)))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not a Component", componentType));
			if (componentType.IsAbstract)
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is abstract", componentType));
		}

		public override void Validate(IReadOnlyCollection<MemberValidatedAttributePair> pairs)
		{
			if (pairs.Count <= 1) return;

			var pairsByComponentType = pairs.GroupBy(pair =>
				((PropertyInfo)pair.Member).PropertyType.GetGenericArguments()[0]);

			foreach (var componentTypePairs in pairsByComponentType)
				if (componentTypePairs.Count() > 1)
					throw new AttributeUsageException(this, null, string.Format(
						"{0} tracked multiple times", componentTypePairs.Key.Name));
		}
	}
}

