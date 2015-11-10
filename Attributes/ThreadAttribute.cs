using System;
using System.Reflection;
using OtherEngine.Utility.Attributes;

namespace OtherEngine.Core.Attributes
{
	/// <summary> Controls in which thread processors should be ticked.
	///           If omitted, uses the default thread, "OtherEngine:Update". </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ThreadAttribute : ValidatedAttribute
	{
		public string Identifier { get; private set; }


		public ThreadAttribute(string identifier)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");

			Identifier = identifier;
		}


		public override void Validate(ICustomAttributeProvider target)
		{
			var type = (Type)target;

			if (!typeof(IProcessor).IsAssignableFrom(type))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not an IProcessor", type));
		}
	}
}

