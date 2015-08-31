using System;
using System.Linq;
using System.Text;

namespace OtherEngine.Core.Utility
{
	public static class TypeUtils
	{
		/// <summary>
		/// Returns if the current type is the same type or a subtype
		/// of the specified type. Also works for interfaces and generic
		/// type definitions.
		/// 
		/// subType.IsSubclassOf(type) should be used instead, if the
		/// subType is known to not be an interface, and type is not
		/// a generic type definition.
		/// </summary>
		public static bool Is(this Type subType, Type type)
		{
			if (subType == null)
				throw new ArgumentNullException("subType");
			if (type == null)
				throw new ArgumentNullException("type");

			// If the specified type is an interface, see
			// if current type implements this interface.
			if (type.IsInterface) {
				if (subType.GetInterfaces().Any(i => IsExact(type, i)))
					return true;
			// An interface can't be a subtype of a class.
			} else if (subType.IsInterface)
				return false;

			// Go through the current type's hierarchy, testing
			// all its base types against the specified type.
			for (; subType != null; subType = subType.BaseType) {
				if (IsExact(type, subType))
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns if the current type is the same type or a subtype
		/// of the generic type. Also works for interfaces.
		/// 
		/// subType.IsSubclassOf(typeof(TType)) should be used instead,
		/// if the subType is known to not be an interface, and type is
		/// not a generic type definition.
		/// </summary>
		public static bool Is<TType>(this Type subType)
		{
			return Is(subType, typeof(TType));
		}

		static bool IsExact(Type type, Type subType)
		{
			return (type == ((type.IsGenericTypeDefinition && subType.IsGenericType)
				? subType.GetGenericTypeDefinition() : subType));
		}


		/// <summary>
		/// Returns the pretty name of a type.
		/// For non-generic types, returns the same as type.Name.
		/// For generic types, returns for example List&lt;string&gt; instead of List`1.
		/// </summary>
		public static string GetPrettyName(this Type type)
		{
			return AppendPrettyName(new StringBuilder(), type).ToString();
		}
		static StringBuilder AppendPrettyName(StringBuilder sb, Type type)
		{
			sb.Append(type.Name);

			var index = type.Name.LastIndexOf('`');
			if (index < 0) return sb;
			sb.Remove(index, sb.Length - index);

			sb.Append('<');
			foreach (var t in type.GetGenericArguments()) {
				AppendPrettyName(sb, t);
				sb.Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);
			sb.Append('>');

			return sb;
		}
	}
}

