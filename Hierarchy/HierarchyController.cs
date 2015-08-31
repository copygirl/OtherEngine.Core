using System;
using OtherEngine.Core.Attributes;

namespace OtherEngine.Core.Hierarchy
{
	/// <summary>
	/// Handles entity hierarchy using the HierarchyComponent.
	/// </summary>
	[AutoEnable]
	public class HierarchyController : Controller
	{
		#region Adding entities

		/// <summary>
		/// Adds a child entity to a parent entity.
		/// 
		/// This creates HierarchyComponents if necessary.
		/// Throws an exception if the child entity already has a parent.
		/// </summary>
		public void Add(Entity parent, Entity child)
		{
			HierarchyComponent childHier, parentHier;
			GetHierarchies(parent, child, out childHier, out parentHier);

			parentHier._children.Add(child);
			childHier.Parent = parent;
		}

		/// <summary>
		/// Adds a labeled child entity to a parent entity,
		/// so it can be looked up using that label.
		/// 
		/// This creates HierarchyComponents if necessary.
		/// Throws an exception if the child entity already has a parent.
		/// Throws an exception if the parent already uses that label.
		/// </summary>
		public void Add(Entity parent, string label, Entity child)
		{
			HierarchyComponent childHier, parentHier;
			GetHierarchies(parent, child, out childHier, out parentHier);

			if (label == null)
				throw new ArgumentNullException("label");

			try { parentHier._labeledChildren.Add(label, child); }
			catch (Exception ex) {
				throw new ArgumentException(string.Format(
					"{0} is already using the label '{1}'", parent, label), ex);
			}

			child.Add(new HierarchyLabelComponent(label));
			childHier.Parent = parent;
		}

		static void GetHierarchies(Entity parent, Entity child,
			out HierarchyComponent childHier, out HierarchyComponent parentHier)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (child == null)
				throw new ArgumentNullException("child");

			childHier = child.GetOrCreate<HierarchyComponent>();
			if (childHier.Parent != null)
				throw new ArgumentException(string.Format(
					"{0} is already the child entity of {1}",
					child, childHier.Parent), "child");

			parentHier = parent.GetOrCreate<HierarchyComponent>();
		}

		#endregion

		#region Removing entities

		/// <summary>
		/// Removes the child entity from its parent.
		/// </summary>
		public void Remove(Entity child)
		{
			if (child == null)
				throw new ArgumentNullException("child");
			
			var childHier = child.Get<HierarchyComponent>();
			if ((childHier == null) || (childHier.Parent == null))
				throw new ArgumentException(string.Format(
					"{0} doesn't have a parent", child), "child");

			var parentHier = childHier.Parent.GetOrThrow<HierarchyComponent>();

			if (!parentHier._children.Remove(child)) {
				string label = child.Get<HierarchyLabelComponent>()?.Value;
				parentHier._labeledChildren.Remove(label);
			}

			childHier.Parent = null;
		}

		#endregion
	}
}

