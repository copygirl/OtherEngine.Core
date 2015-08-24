using System;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Attached to special controller container entities, which
	/// contain additional information related to these controllers.
	/// Used by the ControllerController.
	/// </summary>
	public class ControllerComponent : Component
	{
		/// <summary>
		/// Gets the type of the controller for this container entity.
		/// </summary>
		public Type ControllerType { get; private set; }

		/// <summary>
		/// Gets the controller for this container entity.
		/// May be null if the controller hasn't been instantiated yet.
		/// </summary>
		public Controller Controller { get; internal set; }


		internal ControllerComponent(Type controllerType)
		{
			ControllerType = controllerType;
		}
	}
}

