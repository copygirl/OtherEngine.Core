using System;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// A <see cref="GameSystem"/> class with this attribute will be
	/// automatically enabled when the module (assembly) that contains
	/// it is loaded using <see cref="CoreModuleHandler.LoadModule"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AutoEnableAttribute : Attribute
	{
	}
}

