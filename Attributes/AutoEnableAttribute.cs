using System;
using System.Reflection;
using OtherEngine.Utility.Attributes;

namespace OtherEngine.Core.Attributes
{
	/// <summary> Auto-enables processors when their module is loaded. </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoEnableAttribute : ValidatedAttribute
	{
		public override void Validate(ICustomAttributeProvider target)
		{
			var type = (Type)target;
			if (!typeof(IProcessor).IsAssignableFrom(type))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} isn't an IProcessor", type));
		}
	}
}

