using System;
using System.Text.RegularExpressions;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core
{
	/// <summary>
	/// Takes care of the game's logic by handling
	/// and updating a few components at a time.
	/// </summary>
	public abstract class Controller
	{
		/// <summary>
		/// Gets the game instance this controller was created for.
		/// May be null if the controller has not been created using a game instance.
		/// </summary>
		public Game Game { get; internal set; }

		/// <summary>
		/// Gets the state of the controller.
		/// This is controlled by the ControllerController.
		/// </summary>
		public ControllerState State { get; internal set; }

		/// <summary>
		/// Gets the name of the controller, without the "Controller" suffix.
		/// </summary>
		public string Name { get { return GetName(GetType()); } }


		protected Controller() {  }


		#region OnEnabled / OnDisabled

		/// <summary>
		/// Called when the system is enabled.
		/// </summary>
		protected internal virtual void OnEnabled() {  }

		/// <summary>
		/// Called before the system is disabled.
		/// May be used to clean things up.
		/// </summary>
		protected internal virtual void OnDisabled() {  }

		#endregion

		#region GetName / ToString related

		static readonly Regex _removeControllerRegex = new Regex("Controller(<.*>)?$");

		/// <summary>
		/// Returns the simple name of the component type, without the "Component" suffix.
		/// </summary>
		public static string GetName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsSubclassOf(typeof(Controller)))
				throw new ArgumentException(string.Format(
					"{0} is not a Controller", type), "type");

			return _removeControllerRegex.Replace(type.GetPrettyName(), "$1");
		}

		public static string ToString(Type type)
		{
			return string.Format("[Controller {0}]", GetName(type));
		}

		public sealed override string ToString()
		{
			return ToString(GetType());
		}

		#endregion
	}

	public enum ControllerState
	{
		/// <summary>
		/// The controller state is invalid - it might have been
		/// initialized without being attached to a game object,
		/// or for some reason hasn't been initialized yet.
		/// </summary>
		Invalid,

		/// <summary>
		/// The controller is disabled -
		/// it will not receive any events.
		/// </summary>
		Disabled,

		/// <summary>
		/// The controller is enabled -
		/// it will receive events and do work.
		/// </summary>
		Enabled
	}
}

