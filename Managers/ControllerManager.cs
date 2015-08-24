using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OtherEngine.Core.Components;
using OtherEngine.Core.Controllers;

namespace OtherEngine.Core.Managers
{
	/// <summary>
	/// Handles the creation and lookup of
	/// the game's controllers and containers.
	/// </summary>
	public class ControllerManager : ContainerManager<Controller>, IEnumerable<Controller>
	{
		internal ControllerManager(Game game) : base(game) {  }


		#region Getting controllers

		/// <summary>
		/// Returns a controller of a specific type. Throws an exception
		/// if the module containing the controller is not loaded, or the
		/// controller type cannot be instantiated.
		/// </summary>
		public TController Get<TController>()
			where TController : Controller
		{
			return (TController)Get(typeof(TController));
		}

		/// <summary>
		/// Returns a controller of the specified type. Throws an exception
		/// if the module containing the controller is not loaded, or the
		/// controller type cannot be instantiated.
		/// </summary>
		internal Controller Get(Type controllerType)
		{
			var container = GetContainer(controllerType);

			var component = container.GetOrThrow<ControllerComponent>();
			if (component.Controller == null) {
				component.Controller = Instantiate(controllerType);
				Initialize(component.Controller);
			}

			return component.Controller;
		}

		#endregion

		#region Instantiating and initializing controllers

		Controller Instantiate(Type controllerType)
		{
			if (controllerType.IsAbstract)
				throw new ControllerInstantiationException(controllerType, string.Format(
					"{0} is abstract", controllerType));

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			var constructor = controllerType.GetConstructor(flags, null, Type.EmptyTypes, null);
			if (constructor == null)
				throw new ControllerInstantiationException(controllerType, string.Format(
					"{0} doesn't have a default constructor", controllerType));

			Controller controller;
			try { controller = (Controller)constructor.Invoke(new object[0]); }
			catch (Exception ex) {
				throw new ControllerInstantiationException(controllerType, string.Format(
					"Exception in {0} constructor: {1}", controllerType, ex.Message), ex);
			}

			return controller;
		}

		void Initialize(Controller controller)
		{
			controller.Game = Game;
		}

		#endregion

		#region Enabling and disabling controller

		public void Enable(Controller controller)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");
			if (controller.Game != Game)
				throw new ArgumentException("Controller wasn't instantiated for this game", "controller");
			if (controller.State == ControllerState.Enabled)
				throw new ArgumentException("Controller is already enabled", "controller");
			
			controller.State = ControllerState.Enabled;
			controller.OnEnabled();
			Game.Events.OnControllerEnabled(controller);
		}

		public TController Enable<TController>()
			where TController : Controller
		{
			var controller = Get<TController>();
			Enable(controller);
			return controller;
		}


		public void Disable(Controller controller)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");
			if (controller.Game != Game)
				throw new ArgumentException("Controller wasn't instantiated for this game", "controller");
			if (controller.State == ControllerState.Disabled)
				throw new ArgumentException("Controller is already disabled", "controller");

			controller.State = ControllerState.Disabled;
			controller.OnDisabled();
			Game.Events.OnControllerDisabled(controller);
		}

		public TController Disable<TController>()
			where TController : Controller
		{
			var controller = Get<TController>();
			Disable(controller);
			return controller;
		}

		#endregion


		protected override Entity CreateContainer(Type type)
		{
			var container = base.CreateContainer(type);
			container.Add(new NameComponent{ Value = Controller.GetName(type) });
			container.Add(new ControllerComponent(type));
			return container;
		}


		#region IEnumerable implementation

		public IEnumerator<Controller> GetEnumerator()
		{
			return Containers.Values
				.Select(container => container.GetOrThrow<ControllerComponent>().Controller)
				.Where(controller => (controller != null))
				.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}

