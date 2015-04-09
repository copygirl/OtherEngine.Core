using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace OtherEngine.Core.Utility
{
	public static class AttributeUtils
	{
		public static IEnumerable<TMember> GetAttributes<TMember, TAttribute>(
			this Type type, Func<TMember, TAttribute, bool> predicate = null)
			where TMember : MemberInfo where TAttribute : Attribute
		{
			return type.GetMembers().Where((info) => {
				TAttribute attribute;
				return ((info is TMember) &&
					((attribute = info.GetCustomAttribute<TAttribute>()) != null) &&
					((predicate == null) || predicate((TMember)info, attribute)));
			}).Cast<TMember>();
		}
	}
}

