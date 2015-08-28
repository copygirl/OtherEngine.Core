using System.Collections.Generic;
using System.Linq;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Adds hierarchy information to entities.
	/// Holds the parent and child entities of this entity.
	/// </summary>
	public class HierarchyComponent : Component
	{
		internal readonly HashSet<Entity> _children = new HashSet<Entity>();

		/// <summary>
		/// Gets the parent entity of this entity, null if none.
		/// </summary>
		public Entity Parent { get; internal set; }

		/// <summary>
		/// Gets the child entities of this entity.
		/// </summary>
		public IEnumerable<Entity> Children { get { return _children.Select(c => c); } }
	}
}
