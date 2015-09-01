using System.Collections.Generic;
using OtherEngine.Core.Utility;

namespace OtherEngine.Core.Tracking
{
	/// <summary>
	/// Attached to component containers when at least
	/// one controller is tracking the component type.
	/// </summary>
	public class TrackedEntitiesComponent : Component
	{
		internal HashSet<Controller> _controllers = new HashSet<Controller>();

		/// <summary>
		/// Gets the controllers currently
		/// tracking this component type.
		/// </summary>
		public IReadOnlyCollection<Controller> Controllers { get; private set; }

		/// <summary>
		/// Gets a (readonly) collection of the entities
		/// tracked that have this component type.
		/// </summary>
		public EntityCollection Entities { get; private set; }


		internal TrackedEntitiesComponent(EntityCollection entities)
		{
			Entities = entities;
			Controllers = new ReadOnlyCollectionWrapper<Controller>(_controllers);
		}
	}
}

