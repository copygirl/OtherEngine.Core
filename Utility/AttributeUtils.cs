using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OtherEngine.Core.Utility
{
	public static class AttributeUtils
	{
		#region GetAttributes and GetAttribute

		/// <summary>
		/// Returns all the attribute provider's attributes of type TAttr.
		/// If inherit is true, checks inherited attributes.
		/// </summary>
		public static IEnumerable<TAttr> GetAttributes<TAttr>(
			this ICustomAttributeProvider info, bool inherit = false)
			where TAttr : Attribute
		{
			return info.GetCustomAttributes(typeof(TAttr), inherit).Select(a => (TAttr)a);
		}
		/// <summary>
		/// Returns all the type's attributes of type TAttr,
		/// If inherit is true, checks inherited attributes.
		/// </summary>
		public static IEnumerable<TAttr> GetAttributes<TType, TAttr>(
			bool inherit = false)
			where TAttr : Attribute
		{
			return typeof(TType).GetAttributes<TAttr>(inherit);
		}

		/// <summary>
		/// Returns the attribute provider's attribute of type TAttr,
		/// null if none. Only the first matching attribute is returned.
		/// If inherit is true, checks inherited attributes.
		/// </summary>
		public static TAttr GetAttribute<TAttr>(
			this ICustomAttributeProvider info, bool inherit = false)
			where TAttr : Attribute
		{
			return info.GetAttributes<TAttr>(inherit).FirstOrDefault();
		}
		/// <summary>
		/// Returns the type's attribute of type TAttr, null if
		/// none. Only the first matching attribute is returned.
		/// If inherit is true, checks inherited attributes.
		/// </summary>
		public static TAttr GetAttribute<TType, TAttr>(
			bool inherit = false)
			where TAttr : Attribute
		{
			return typeof(TType).GetAttribute<TAttr>(inherit);
		}

		#endregion

		#region GetMemberAttributes and MemberAttributePair

		/// <summary>
		/// Enumerates all members of the current type to get members of type
		/// TMember which have one or multiple attributes of type TAttr.
		/// </summary>
		public static IEnumerable<MemberAttributePair<TMember, TAttr>> GetMembersWithAttribute<TMember, TAttr>(
			this Type type, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public)
			where TMember : MemberInfo where TAttr : Attribute
		{
			return type.GetMembers(bindingAttr).OfType<TMember>()
				.SelectMany(m => m.GetCustomAttributes<TAttr>()
					.Select(a => new MemberAttributePair<TMember, TAttr>(m, a)));
		}

		#endregion

		#region ICustomAttributeProvider.GetName

		/// <summary>
		/// Gets the name of an attribute target. For example:
		///   [Assembly: OtherEngine.Core],
		///   [Type: TestController],
		///   [Method: TestController.OnEvent],
		///   [Property: TestController.Entities],
		///   [Parameter: TestController.DoSomething input],
		///   [Return: TestController.DoSomething]
		/// </summary>
		public static string GetName(this ICustomAttributeProvider target)
		{
			var sb = new StringBuilder("[");
			if (target is Assembly)
				sb.Append("Assembly: ").Append(((Assembly)target).GetName().Name);
			else if (target is Module)
				sb.Append("Module: ").Append(((Module)target).Name);
			else if (target is Type)
				sb.Append("Type: ").Append(((Type)target).Name);
			else if (target is MemberInfo) {
				var info = (MemberInfo)target;
				switch (info.MemberType) {
					case MemberTypes.Constructor: sb.Append("Constructor: "); break;
					case MemberTypes.Method: sb.Append("Method: "); break;
					case MemberTypes.Field: sb.Append("Field: "); break;
					case MemberTypes.Property: sb.Append("Property: "); break;
					case MemberTypes.Event: sb.Append("Event: "); break;
				}
				sb.Append(info.ReflectedType.Name).Append('.').Append(info.Name);
			} else if (target is ParameterInfo) {
				var info = (ParameterInfo)target;
				sb.Append((info.Position >= 0) ? "Parameter: " : "Return: ");
				sb.Append(info.Member.ReflectedType.Name).Append(".").Append(info.Member.Name);
				if (info.Position >= 0) sb.Append(' ').Append(info.Name);
			} else sb.Append("Unknown Attribute Target");
			return sb.Append(']').ToString();
		}

		#endregion
	}

	/// <summary>
	/// Helper class which represents a MemberInfo / Attribute pair.
	/// </summary>
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
	}
}

