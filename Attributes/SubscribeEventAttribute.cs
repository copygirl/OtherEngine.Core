using System;
using OtherEngine.Core.Events;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Attributes
{
	/// <summary>
	/// A <see cref="GameSystem"/> method with this attribute will be registered
	/// in the <see cref="CoreEventHandler"/> using its parameter as the type of
	/// event being listened to. Only enabled GameSystems will receive events.
	/// 
	/// The method mustn't be static or abstract, must be public and contain a
	/// single parameter of type <see cref="IGameEvent"/> or a (non-abstract)
	/// class based on it.
	/// </summary>
	/// <example>
	/// 	[SubscribeEvent]
	/// 	public void OnExample(ExampleEvent ev) { ... }
	/// </example>
	[AttributeUsage(AttributeTargets.Method)]
	public class SubscribeEventAttribute : Attribute
	{
	}
}

