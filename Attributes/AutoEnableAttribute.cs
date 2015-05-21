using System;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// A <see cref="GameSystem"/> class with this attribute will be automatically
	/// enabled when the <see cref="Game"/> instance is initialized or the assembly
	/// containing the GameSystem type is loaded.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoEnableAttribute : Attribute
	{
	}
}

