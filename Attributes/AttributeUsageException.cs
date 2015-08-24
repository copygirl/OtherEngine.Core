using System;
using System.Reflection;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// Thrown when an attribute is used incorrectly.
	/// </summary>
	public class AttributeUsageException : Exception
	{
		/// <summary>
		/// The attribute being validated.
		/// </summary>
		public Attribute Attribute { get; private set; }

		/// <summary>
		/// The target the attribute is attached to.
		/// May be null if the problem is not regarding a single target.
		/// </summary>
		public ICustomAttributeProvider Target { get; private set; }


		public AttributeUsageException(Attribute attribute, ICustomAttributeProvider target, string message)
			: base(string.Format("Error validating {0}: {1}", attribute?.GetType().Name, message))
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");
			if (message == null)
				throw new ArgumentNullException("message");

			Attribute = attribute;
			Target = target;
		}
	}
}
