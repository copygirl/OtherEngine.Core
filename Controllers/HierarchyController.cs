using System;
using System.Collections.Generic;
using System.Linq;
using OtherEngine.Core.Components;

namespace OtherEngine.Core.Controllers
{
	/// <summary>
	/// Handles entity hierarchy using the HierarchyComponent.
	/// </summary>
	public class HierarchyController : Controller
	{
		/// <summary>
		/// Adds a child entity to a parent entity.
		/// This creates HierarchyComponents if necessary.
		/// Throws an exception if the child entity already has a parent.
		/// </summary>
		public void Add(Entity parent, Entity child)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (child == null)
				throw new ArgumentNullException("child");

			var childHier = child.GetOrCreate<HierarchyComponent>();
			if (childHier.Parent != null)
				throw new InvalidOperationException(string.Format(
					"{0} is already the child entity of {1}", child, childHier.Parent));

			var parentHier = parent.GetOrCreate<HierarchyComponent>();

			parentHier._children.Add(child);
			childHier.Parent = parent;
		}

		/// <summary>
		/// Removes the child entity from its parent.
		/// </summary>
		public void Remove(Entity child)
		{
			if (child == null)
				throw new ArgumentNullException("child");
			
			var childHier = child.Get<HierarchyComponent>();
			if ((childHier == null) || (childHier.Parent == null))
				throw new InvalidOperationException(string.Format(
					"{0} doesn't have a parent", child));

			var parentHier = childHier.Parent.GetOrThrow<HierarchyComponent>();

			parentHier._children.Remove(child);
			childHier.Parent = null;
		}
	}

	public static class EntityHierarchyExtensions
	{
		#region Get parent / children

		/// <summary>
		/// Returns the parent of this entity, null if none.
		/// </summary>
		public static Entity GetParent(this Entity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			return entity.Get<HierarchyComponent>()?.Parent;
		}

		/// <summary>
		/// Returns the children of this entity, null if none.
		/// </summary>
		public static IEnumerable<Entity> GetChildren(this Entity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			return entity.Get<HierarchyComponent>()?.Children ?? Enumerable.Empty<Entity>();
		}

		/// <summary>
		/// Returns first child of this entity, null if none.
		/// </summary>
		public static Entity GetChild(this Entity entity)
		{
			return entity.GetChildren().FirstOrDefault();
		}

		#endregion

		#region Adding / removing child entities

		/// <summary>
		/// Adds a child entity to the parent entity.
		/// </summary>
		public static void Add(this Entity parent, Entity child)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (child == null)
				throw new ArgumentNullException("child");

			parent.Game.Controllers.Get<HierarchyController>().Add(parent, child);
		}

		/// <summary>
		/// Adds child entities to the parent entity.
		/// </summary>
		public static void Add(this Entity parent, params Entity[] children)
		{
			if (children == null)
				throw new ArgumentNullException("children");
			if (children.Length <= 0)
				throw new ArgumentException("children doesn't contain any elements", "children");
			if (children.Contains(null))
				throw new ArgumentException("children contains null elements", "children");
			
			foreach (var child in children)
				parent.Add(child);
		}

		/// <summary>
		/// Removes the child entity from its parent.
		/// </summary>
		public static void Remove(this Entity child)
		{
			if (child == null)
				throw new ArgumentNullException("child");
			
			child.Game.Controllers.Get<HierarchyController>().Remove(child);
		}

		#endregion

		#region Adding / getting group entities

		/// <summary>
		/// Adds a group entity with the specified name to the parent.
		/// </summary>
		public static Entity Add(this Entity parent, string groupName)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (groupName == null)
				throw new ArgumentNullException("groupName");

			if (parent.Get(groupName) != null)
				throw new ArgumentException(string.Format(
					"[Group \"{0}\"] already exists in {1}", groupName, parent), "groupName");
			
			var child = new Entity(parent.Game) {
				new GroupComponent(),
				new TypeComponent { Value = "Group" },
				new NameComponent { Value = groupName }
			};
			parent.Add(child);

			return child;
		}

		/// <summary>
		/// Returns the group entities of this entity.
		/// </summary>
		public static IEnumerable<Entity> GetGroups(this Entity entity)
		{
			return entity.GetChildren().Where(child =>
				(child.Get<GroupComponent>() != null));
		}

		/// <summary>
		/// Returns the group with the specified name of this entity, null if none.
		/// </summary>
		public static Entity Get(this Entity entity, string groupName)
		{
			return entity.GetGroups().FirstOrDefault(child =>
				(child.Get<NameComponent>()?.Value == groupName));
		}


		/// <summary>
		/// Private component to identify group entities.
		/// </summary>
		class GroupComponent : Component
		{
		}

		#endregion
	}
}

