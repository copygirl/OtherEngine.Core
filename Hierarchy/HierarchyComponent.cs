using System.Collections.Generic;
using System.Linq;

namespace OtherEngine.Core.Hierarchy
{
	/// <summary>
	/// Adds hierarchy information to entities.
	/// Holds the parent and child entities of this entity.
	/// </summary>
	public class HierarchyComponent : Component
	{
		internal readonly HashSet<Entity> _children = new HashSet<Entity>();
		internal readonly Dictionary<string, Entity> _labeledChildren = new Dictionary<string, Entity>();

		/// <summary>
		/// Gets the parent entity of this entity, null if none.
		/// </summary>
		public Entity Parent { get; internal set; }

		/// <summary>
		/// Gets the child entities of this entity.
		/// </summary>
		public IEnumerable<Entity> Children { get { return _labeledChildren.Values.Concat(_children); } }

		/// <summary>
		/// Gets a child entity of this entity with this label, null if none.
		/// </summary>
		public Entity this[string label] {
			get {
				Entity entity;
				return (_labeledChildren.TryGetValue(label, out entity) ? entity : null);
			}
		}
	}
}

