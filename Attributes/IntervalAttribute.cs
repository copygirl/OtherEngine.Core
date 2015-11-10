using System;
using System.Reflection;
using OtherEngine.Utility.Attributes;
using OtherEngine.ES;

namespace OtherEngine.Core.Attributes
{
	/// <summary> Controls how often processors get ticked.
	///           If omitted, uses the thread's default interval. </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class IntervalAttribute : ValidatedAttribute
	{
		/// <summary> Gets the interval at which the processor should be ticked. </summary>
		public GameTime Interval { get; private set; }


		public IntervalAttribute(GameTime interval)
		{
			if (interval <= GameTime.Zero)
				throw new ArgumentException(string.Format(
					"Interval needs to be positive ({0})", interval), "interval");

			Interval = interval;
		}


		public override void Validate(ICustomAttributeProvider target)
		{
			var type = (Type)target;
			if (!typeof(IProcessor).IsAssignableFrom(type))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} isn't an IProcessor", type));
		}
	}
}

