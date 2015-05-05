using System;
using System.Linq;

namespace OtherEngine.Core.Utility
{
	public static class TypeUtils
	{
		public static string ToString(Type type)
		{
			return type.ToString();
		}
		public static string ToString(object obj)
		{
			return ToString(obj.GetType());
		}
		public static string ToString<T>()
		{
			return ToString(typeof(T));
		}

		// FIXME: Probably not needed anymore, keeping just in case.
		/// <summary>
		/// Returns if the generic parameters fit the constraints of the generic type definition.
		/// Note that this doesn't check generic attributes, like new().
		/// </summary>
		public static bool IsValidGenericArguments(this Type type, params Type[] genericParameters)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsGenericTypeDefinition)
				throw new ArgumentException(String.Format(
					"{0} is not a generic type definition", type), "type");
			
			if (genericParameters == null)
				throw new ArgumentNullException("genericParameters");
			if (type.GetGenericArguments().Length != genericParameters.Length)
				throw new ArgumentException(String.Format(
					"Not the right amount of generic parameters (expected {0}, got {1})",
					type.GetGenericArguments().Length, genericParameters.Length));
			
			return (type.GetGenericArguments()
				.Zip(genericParameters, (t, p) => new { Type = t, GenericParameter = p })
				.All(i => i.Type.GetGenericParameterConstraints()
					.All(t => t.IsAssignableFrom(i.GenericParameter))));
		}
	}
}

