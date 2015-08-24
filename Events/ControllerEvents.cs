
namespace OtherEngine.Core.Events
{
	/// <summary>
	/// Base class for other controller events.
	/// </summary>
	public abstract class ControllerEvent : Event
	{
		public Controller Controller { get; private set; }

		protected ControllerEvent(Controller controller)
		{
			Controller = controller;
		}
	}

	/// <summary>
	/// Fired when a controller is enabled.
	/// 
	/// Note that if a controller is subscribed to this and is,
	/// enabled it will receive the enabled event for itself.
	/// </summary>
	public class ControllerEnabledEvent : ControllerEvent
	{
		internal ControllerEnabledEvent(Controller controller)
			: base(controller) {  }
	}

	/// <summary>
	/// Fired when a controller is disabled.
	/// 
	/// Unlike the enabled event, controllers won't
	/// receive the disabled event for themselves.
	/// </summary>
	public class ControllerDisabledEvent : ControllerEvent
	{
		internal ControllerDisabledEvent(Controller controller)
			: base(controller) {  }
	}
}

