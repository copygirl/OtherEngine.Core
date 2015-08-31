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
		public Entity this[string label] { get {
				Entity entity;
				return (_labeledChildren.TryGetValue(label, out entity) ? entity : null);
			} }
	}

	/// <summary>
	/// Added to entities when they're added to the
	/// hierarchy of another entity using a label.
	/// </summary>
	public class HierarchyLabelComponent : SimpleComponent<string>
	{
		internal HierarchyLabelComponent(string label)
			: base(label) {  }
	}

	/// <summary>
	/// Component to identify group entities.
	/// Group entities use their NameComponent as the name for the group.
	/// </summary>
	public class HierarchyGroupComponent : Component
	{
		internal HierarchyGroupComponent() {  }
	}

	/// <summary>
	/// Component to identify hierarchy link entities.
	/// Link entities use their TargetComponent for the entity they're linked to.
	/// </summary>
	public class HierarchyLinkComponent : Component
	{
		internal HierarchyLinkComponent() {  }
	}
}
