using System;
using System.Reflection;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// A controller class with this attribute will be automatically
	/// enabled when the module that contains it is loaded.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoEnableAttribute : ValidatedAttribute
	{
		public override void Validate(ICustomAttributeProvider target)
		{
			var controllerType = (Type)target;
			if (!controllerType.Is<Controller>())
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not a Controller", target.GetName()));

			if (controllerType.IsAbstract)
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is abstract", target.GetName()));

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var constructor = controllerType.GetConstructor(flags, null, Type.EmptyTypes, null);
			if (constructor == null)
				throw new AttributeUsageException(this, target, string.Format(
					"{0} doesn't have a default constructor", target.GetName()));
		}
	}
}

