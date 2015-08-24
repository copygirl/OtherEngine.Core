using System;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Utility;
using System.Collections.Generic;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// Base class for attributes that can be validated.
	/// When an attribute is based on this class, it should be tested for validity
	/// automatically. A sucessfully validated attribute should be usable as intended.
	/// </summary>
	public abstract class ValidatedAttribute : Attribute
	{
		/// <summary>
		/// Called to validate the usage of the attribute.
		/// Should throw an AttributeUsageException if something's wrong.
		/// </summary>
		public abstract void Validate(ICustomAttributeProvider target);

		/// <summary>
		/// Called to validate the usage of attributes within a type.
		/// Only called once on one of the attributes of this type,
		/// after all attributes have been validated seperately.
		/// </summary>
		public virtual void Validate(IReadOnlyCollection<MemberValidatedAttributePair> pairs) {  }


		/// <summary>
		/// Validates all attributes on this type and its members.
		/// </summary>
		public static void ValidateAll(Type type)
		{
			foreach (var attr in type.GetAttributes<ValidatedAttribute>())
				attr.Validate(type);

			foreach (var member in type.GetMembers())
				foreach (var attr in member.GetAttributes<ValidatedAttribute>())
					attr.Validate(member);

			// Iterate all members, create member-attribute pairs and
			// group them by the attribute type, then validate them.
			var groups = type.GetMembers()
				.SelectMany(member => member
					.GetAttributes<ValidatedAttribute>()
					.Select(attr => new MemberValidatedAttributePair(member, attr)))
				.GroupBy(pair => pair.Attribute.GetType());

			foreach (var group in groups)
				group.First().Attribute.Validate(group.ToList());
		}


		public class MemberValidatedAttributePair : MemberAttributePair<MemberInfo, ValidatedAttribute>
		{
			public MemberValidatedAttributePair(MemberInfo member, ValidatedAttribute attribute)
				: base(member, attribute) {  }
		}
	}
}

