using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OtherEngine.Core.Exceptions;

namespace OtherEngine.Core.Utility
{
	public static class AttributeUtils
	{
		static readonly Regex _matchTrailingAttribute = new Regex("Attribute$");

		public static string GetFriendlyName(Type attributeType)
		{
			if (attributeType == null)
				throw new ArgumentNullException("attributeType");
			return _matchTrailingAttribute.Replace(attributeType.Name, "");
		}
		public static string GetFriendlyName(this Attribute attribute)
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");
			return GetFriendlyName(attribute.GetType());
		}

		/// <summary>
		/// Enumerates all members of to get every member of type TMember which has one or multiple attributes of type TAttr.
		/// If multiple attributes are encountered, multiple <see cref="MemberAttributePair&lt;TMember, TAttr&gt;"/>s are returned.
		/// </summary>
		public static IEnumerable<MemberAttributePair<TMember, TAttr>> GetMemberAttributes<TMember, TAttr>(
			this Type type, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public)
			where TMember : MemberInfo where TAttr : Attribute
		{
			return type.GetMembers(bindingAttr).OfType<TMember>()
				.SelectMany(m => m.GetCustomAttributes<TAttr>()
					.Select(a => new MemberAttributePair<TMember, TAttr>(m, a)));
		}
	}

	public class MemberAttributePair<TMember, TAttr>
		where TMember : MemberInfo where TAttr : Attribute
	{
		public TMember Member { get; private set; }
		public TAttr Attribute { get; private set; }

		public MemberAttributePair(TMember member, TAttr attribute)
		{
			Member = member;
			Attribute = attribute;
		}

		public AttributeException MakeException(Type type, Exception innerException, string format, params object[] args)
		{
			var list = new List<object>(2 + args.Length){ type.FullName, Member.Name };
			list.AddRange(args);
			return new AttributeException<TMember>(Attribute, Member, String.Format(
				"[{0}]: {1}", Attribute.GetFriendlyName(), string.Format(format, list.ToArray())), innerException);
		}
		public AttributeException MakeException(Type type, string format, params object[] args)
		{
			return MakeException(type, null, format, args);
		}
		public AttributeException MakeException(Type type, Exception innerException)
		{
			return MakeException(type, innerException, "Exception with {0}.{1}: {2}", innerException.Message);
		}
	}
}

