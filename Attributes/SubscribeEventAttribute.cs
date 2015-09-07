using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// May be attached to a controller method or property.
	/// A single event type may only be subscribed to once per controller.
	/// 
	/// If the target is a method, this method will get called
	/// immediately after the event is fired.
	/// 
	/// If the target is a property, events will be stored in
	/// a queue and can be enumerated at any point in time.
	/// </summary>
	/// <example>
	/// 	[SubscribeEvent]
	/// 	void OnSomeEvent(SomeEvent ev) { ... }
	/// or
	///     [SubscribeEvent]
	///     IEnumerable<SomeEvent> SomeEventQueue { get; set; }
	/// </example>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public class SubscribeEventAttribute : ValidatedAttribute
	{
		public override void Validate(ICustomAttributeProvider target)
		{
			if (!((MemberInfo)target).DeclaringType.IsSubclassOf(typeof(Controller)))
				throw new AttributeUsageException(this, target, string.Format(
					"{0} is not declared on a Controller", target.GetName()));

			// Get the event type for this target (method or property).
			// Throws an exception if there's something wrong with the
			// definition of that member.
			var eventType = GetEventType(target);

			if (eventType.IsAbstract)
				throw new AttributeUsageException(this, target, string.Format(
					"{0} uses abstract event type {1}", target.GetName(), eventType));
		}

		public override void Validate(IReadOnlyCollection<MemberValidatedAttributePair> pairs)
		{
			if (pairs.Count <= 1) return;

			var pairsByEventType = pairs.GroupBy(pair =>
				((SubscribeEventAttribute)pair.Attribute).GetEventType(pair.Member));

			foreach (var eventTypePairs in pairsByEventType)
				if (eventTypePairs.Count() > 1)
					throw new AttributeUsageException(this, null, string.Format(
						"{0} subscribed to by multiple members", eventTypePairs.Key.Name));
		}

		#region GetEventType

		Type GetEventType(ICustomAttributeProvider target)
		{
			return ((target is MethodInfo)
				? GetEventType((MethodInfo)target)
				: GetEventType((PropertyInfo)target));
		}

		Type GetEventType(MethodInfo method)
		{
			Type eventType;
			if ((method.ReturnType != typeof(void)) ||
				(method.GetParameters().Length != 1) ||
				!(eventType = method.GetParameters()[0].ParameterType).Is<Event>())
				throw new AttributeUsageException(this, method, string.Format(
					"{0} is not a void (~Event)", method.GetName()));
			return eventType;
		}

		Type GetEventType(PropertyInfo property)
		{
			Type eventType;
			if (!property.PropertyType.Is(typeof(IEnumerable<>)))
				throw new AttributeUsageException(this, property, string.Format(
					"{0} is not an IEnumerable<~Event>", property.GetName()));
			eventType = property.PropertyType.GetGenericArguments()[0];
			return eventType;
		}

		#endregion
	}
}

