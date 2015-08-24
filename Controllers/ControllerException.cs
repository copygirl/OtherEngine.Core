using System;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Controllers
{
	/// <summary>
	/// Thrown when there is a problem with a controller or controller type.
	/// </summary>
	public class ControllerException : Exception
	{
		/// <summary>
		/// The type of the controller that caused the exception.
		/// </summary>
		public Type ControllerType { get; private set; }

		/// <summary>
		/// The controller instance that caused the exception.
		/// May be null in cases where the instance is not available.
		/// </summary>
		public Controller Controller { get; private set; }


		public ControllerException(Type controllerType, string message, Exception innerException = null)
			: base(message, innerException)
		{
			if (controllerType == null)
				throw new ArgumentNullException("controllerType");
			if (!controllerType.Is<Controller>() || (controllerType == typeof(Controller)))
				throw new ArgumentException(string.Format(
					"{0} is not a controller", controllerType), "controllerType");

			ControllerType = controllerType;
			Controller = null;
		}

		public ControllerException(Controller controller, string message, Exception innerException = null)
			: base(message, innerException)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");

			ControllerType = controller.GetType();
			Controller = controller;
		}
	}

	/// <summary>
	/// Thrown when there was a problem when instanciating a controller.
	/// </summary>
	public class ControllerInstantiationException : ControllerException
	{
		internal ControllerInstantiationException(Type controllerType, string message, Exception innerException = null)
			: base(controllerType, message, innerException) {  }
	}

	/// <summary>
	/// Thrown when there was a problem when initializing a controller.
	/// </summary>
	public class ControllerInitializationException : ControllerException
	{
		internal ControllerInitializationException(Controller controller, string message, Exception innerException = null)
			: base(controller, message, innerException) {  }
	}
}

