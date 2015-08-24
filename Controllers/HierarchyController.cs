using System;
using OtherEngine.Core.Components;

namespace OtherEngine.Core.Controllers
{
	public class HierarchyController : Controller
	{
		public void Add(Entity parent, Entity child)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (child == null)
				throw new ArgumentNullException("child");

			var childHier = child.GetOrCreate<HierarchyComponent>();
			if (childHier.Parent != null)
				throw new InvalidOperationException(string.Format(
					"{0} is already {1}'s child entity", child, parent));

			var parentHier = parent.GetOrCreate<HierarchyComponent>();

			parentHier._children.Add(child);
			childHier.Parent = parent;
		}

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
}

