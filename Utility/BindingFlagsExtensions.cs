using System.Reflection;

namespace OtherEngine.Core.Utility
{
	public static class BindingFlagsExtensions
	{
		public static BindingFlags IgnoreCase(this BindingFlags flags) { return (flags | BindingFlags.IgnoreCase); }
		public static BindingFlags DeclaredOnly(this BindingFlags flags) { return (flags | BindingFlags.DeclaredOnly); }
		public static BindingFlags Instance(this BindingFlags flags) { return (flags | BindingFlags.Instance); }
		public static BindingFlags Static(this BindingFlags flags) { return (flags | BindingFlags.Static); }
		public static BindingFlags Public(this BindingFlags flags) { return (flags | BindingFlags.Public); }
		public static BindingFlags NonPublic(this BindingFlags flags) { return (flags | BindingFlags.NonPublic); }
		public static BindingFlags FlattenHierarchy(this BindingFlags flags) { return (flags | BindingFlags.FlattenHierarchy); }
		public static BindingFlags InvokeMethod(this BindingFlags flags) { return (flags | BindingFlags.InvokeMethod); }
		public static BindingFlags CreateInstance(this BindingFlags flags) { return (flags | BindingFlags.CreateInstance); }
		public static BindingFlags GetField(this BindingFlags flags) { return (flags | BindingFlags.GetField); }
		public static BindingFlags SetField(this BindingFlags flags) { return (flags | BindingFlags.SetField); }
		public static BindingFlags GetProperty(this BindingFlags flags) { return (flags | BindingFlags.GetProperty); }
		public static BindingFlags SetProperty(this BindingFlags flags) { return (flags | BindingFlags.SetProperty); }
		public static BindingFlags PutDispProperty(this BindingFlags flags) { return (flags | BindingFlags.PutDispProperty); }
		public static BindingFlags PutRefDispProperty(this BindingFlags flags) { return (flags | BindingFlags.PutRefDispProperty); }
		public static BindingFlags ExactBinding(this BindingFlags flags) { return (flags | BindingFlags.ExactBinding); }
		public static BindingFlags SuppressChangeType(this BindingFlags flags) { return (flags | BindingFlags.SuppressChangeType); }
		public static BindingFlags OptionalParamBinding(this BindingFlags flags) { return (flags | BindingFlags.OptionalParamBinding); }
		public static BindingFlags IgnoreReturn(this BindingFlags flags) { return (flags | BindingFlags.IgnoreReturn); }
	}
}

