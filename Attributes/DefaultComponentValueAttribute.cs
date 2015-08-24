using System;
using System.Reflection;
using OtherEngine.Core.Components;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Attributes
{
	// TODO: Have a way to look up the default value.

	/// <summary>
	/// Default value for SimpleComponents used when
	/// the component is missing from the entity.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DefaultComponentValueAttribute : ValidatedAttribute
	{
		public object Value { get; private set; }

		public DefaultComponentValueAttribute(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			Value = value;
		}

		public override void Validate(ICustomAttributeProvider target)
		{
			var type = (Type)target;
			if (!type.Is(typeof(SimpleComponent<>)))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not a SimpleComponent<>", target.GetName()));

			var simpleComponentType = type;
			while (!simpleComponentType.IsGenericType ||
				(simpleComponentType.GetGenericTypeDefinition() != typeof(SimpleComponent<>)))
				simpleComponentType = simpleComponentType.BaseType;

			var defaultType = simpleComponentType.GetGenericArguments()[0];
			if (!defaultType.IsAssignableFrom(Value.GetType()))
				throw new AttributeUsageException(this, target, string.Format(
					"Incompatible default value and SimpleComponent types ({0} and {1}) on {2}",
					Value.GetType(), defaultType, target.GetName()));
		}
	}
}

