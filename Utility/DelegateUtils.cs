using System;
using System.Reflection;

namespace OtherEngine.Core.Utility
{
	public static class DelegateUtils
	{
		/// <summary>
		/// Generic version of method.CreateDelegate(typeof(T)).
		/// </summary>
		public static T CreateDelegate<T>(this MethodInfo method) where T : class
		{
			if (method == null)
				throw new ArgumentNullException("method");
			return method.CreateDelegate(typeof(T)) as T;
		}

		/// <summary>
		/// Generic version of method.CreateDelegate(typeof(T), target).
		/// </summary>
		public static T CreateDelegate<T>(this MethodInfo method, object target) where T : class
		{
			if (method == null)
				throw new ArgumentNullException("method");
			return method.CreateDelegate(typeof(T), target) as T;
		}


		#region Property related

		/// <summary>
		/// Creates a Func&lt;TType, TValue&gt; that, when called, returns the
		/// value of the property for the instance supplied to the function.
		/// </summary>
		public static Func<TType, TValue> MakePropertyGetter<TType, TValue>(
			this PropertyInfo property, bool nonPublic = false)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			if (!property.CanRead)
				throw new ArgumentException("Property doesn't have a getter");
			if (!typeof(TType).Is(property.DeclaringType))
				throw new ArgumentException(string.Format(
					"Type {0} is not compatible with property's declaring type {1}.{2}",
					typeof(TType), property.DeclaringType.GetPrettyName(), property.Name), "TType");
			
			return property.GetGetMethod(nonPublic).CreateDelegate<Func<TType, TValue>>();
		}

		/// <summary>
		/// Creates a Func&lt;TValue&gt; that, when called, returns the
		/// value of the property for the specified target instance.
		/// </summary>
		public static Func<TValue> MakePropertyGetter<TValue>(
			this PropertyInfo property, object target, bool nonPublic = false)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			if (target == null)
				throw new ArgumentNullException("target");
			if (!property.CanRead)
				throw new ArgumentException("Property doesn't have a getter");
			if (!target.GetType().Is(property.DeclaringType))
				throw new ArgumentException(string.Format(
					"Type {0} is not compatible with property's declaring type {1}.{2}",
					target.GetType(), property.DeclaringType.GetPrettyName(), property.Name), "target");
			
			return property.GetGetMethod(nonPublic).CreateDelegate<Func<TValue>>(target);
		}


		/// <summary>
		/// Creates a Action&lt;TType, TValue&gt; that, when called, sets the
		/// value of the property for the instance supplied to the function.
		/// </summary>
		public static Action<TType, TValue> MakePropertySetter<TType, TValue>(
			this PropertyInfo property, bool nonPublic = false)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			if (!property.CanWrite)
				throw new ArgumentException("Property doesn't have a setter");
			if (!typeof(TType).Is(property.DeclaringType))
				throw new ArgumentException(string.Format(
					"Type {0} is not compatible with property's declaring type {1}.{2}",
					typeof(TType), property.DeclaringType.GetPrettyName(), property.Name), "TType");
			
			return property.GetSetMethod(nonPublic).CreateDelegate<Action<TType, TValue>>();
		}

		/// <summary>
		/// Creates a Action&lt;TValue&gt; that, when called, sets the
		/// value of the property for the specified target instance.
		/// </summary>
		public static Action<TValue> MakePropertySetter<TValue>(
			this PropertyInfo property, object target, bool nonPublic = false)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			if (target == null)
				throw new ArgumentNullException("target");
			if (!property.CanWrite)
				throw new ArgumentException("Property doesn't have a setter");
			if (!target.GetType().Is(property.DeclaringType))
				throw new ArgumentException(string.Format(
					"Type {0} is not compatible with property's declaring type {1}.{2}",
					target.GetType(), property.DeclaringType.GetPrettyName(), property.Name), "target");
			
			return property.GetSetMethod(nonPublic).CreateDelegate<Action<TValue>>(target);
		}

		#endregion
	}
}

