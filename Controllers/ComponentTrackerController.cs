using System;
using OtherEngine.Core.Attributes;
using OtherEngine.Core.Events;

namespace OtherEngine.Core.Controllers
{
	[AutoEnable]
	public class ComponentTrackerController : Controller
	{
		void OnControllerEnabled(ControllerEnabledEvent ev)
		{

		}

		void OnControllerDisabled(ControllerDisabledEvent ev)
		{

		}

		void OnComponentAdded(ComponentAddedEvent ev)
		{
			
		}

		void OnComponentRemoved(ComponentRemovedEvent ev)
		{

		}
	}
}

