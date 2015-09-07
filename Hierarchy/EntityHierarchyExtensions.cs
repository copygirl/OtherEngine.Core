using System;
using System.Collections.Generic;
using System.Linq;
using OtherEngine.Core.Components;

namespace OtherEngine.Core.Hierarchy
{
	public static class EntityHierarchyExtensions
	{
		#region Get parent / children / label

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
		/// Returns the children of this entity.
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

		/// <summary>
		/// Returns the child with this label of this entity, null if none.
		/// </summary>
		public static Entity GetChild(this Entity entity, string label)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			return entity.Get<HierarchyComponent>()?[label];
		}

		/// <summary>
		/// Returns this entity's label, null if none.
		/// </summary>
		public static string GetLabel(this Entity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			return entity.Get<HierarchyLabelComponent>()?.Value;
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

			parent.Game.Controllers.Get<HierarchyController>().Add(parent, child);
		}

		/// <summary>
		/// Adds a labeled child entity to the parent entity.
		/// </summary>
		public static void Add(this Entity parent, string label, Entity child)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			parent.Game.Controllers.Get<HierarchyController>().Add(parent, label, child);
		}

		/// <summary>
		/// Adds child entities to the parent entity.
		/// </summary>
		public static void Add(this Entity parent, IEnumerable<Entity> children)
		{
			if (children == null)
				throw new ArgumentNullException("children");

			foreach (var child in children) {
				if (child == null)
					throw new ArgumentException("children contains null elements", "children");
				parent.Add(child);
			}
		}
		/// <summary>
		/// Adds child entities to the parent entity.
		/// </summary>
		public static void Add(this Entity parent, params Entity[] children)
		{
			parent.Add(children.AsEnumerable());
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

		#region Adding group entities

		/// <summary>
		/// Adds a (labeled) group entity with the specified name to the parent.
		/// </summary>
		public static Entity AddGroup(this Entity parent, string groupName)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (groupName == null)
				throw new ArgumentNullException("groupName");

			var child = new Entity(parent.Game) {
				new HierarchyGroupComponent(),
				new TypeComponent { Value = "Group" },
				new NameComponent { Value = groupName }
			};
			parent.Add(groupName, child);

			return child;
		}
		/// <summary>
		/// Adds a (labeled) group entity with the specified name to the parent.
		/// </summary>
		public static Entity Add(this Entity parent, string groupName)
		{
			return parent.AddGroup(groupName);
		}

		/// <summary>
		/// Adds a (labeled) group entity with the specified name and child entities to the parent.
		/// </summary>
		public static Entity AddGroup(this Entity parent, string groupName, IEnumerable<Entity> children)
		{
			var group = parent.Add(groupName);
			group.Add(children);
			return group;
		}
		/// <summary>
		/// Adds a (labeled) group entity with the specified name and child entities to the parent.
		/// </summary>
		public static Entity AddGroup(this Entity parent, string groupName, params Entity[] children)
		{
			return parent.AddGroup(groupName, children.AsEnumerable());
		}

		#endregion

		#region Adding / getting linked entities

		/// <summary>
		/// Adds a link entity which points to linkedEntity,
		/// without modifying the hierarchy of the linkedEntity.
		/// </summary>
		public static Entity AddLink(this Entity parent, Entity linkedEntity)
		{
			var link = CreateLinkEntity(parent, linkedEntity);
			parent.Add(link);
			return link;
		}
		/// <summary>
		/// Adds a labeled link entity which points to linkedEntity,
		/// without modifying the hierarchy of the linkedEntity.
		/// </summary>
		public static Entity AddLink(this Entity parent, string label, Entity linkedEntity)
		{
			var link = CreateLinkEntity(parent, linkedEntity);
			parent.Add(label, link);
			return link;
		}
		static Entity CreateLinkEntity(Entity parent, Entity linkedEntity)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (linkedEntity == null)
				throw new ArgumentNullException("linkedEntity");
			if (linkedEntity.Has<HierarchyLinkComponent>())
				throw new ArgumentException(string.Format(
					"Cannot create link entity to another link entity ({0})", linkedEntity), "linkedEntity");

			return new Entity(parent.Game) {
				new HierarchyLinkComponent(),
				new TypeComponent { Value = "Link" },
				new TargetComponent { Value = linkedEntity }
			};
		}

		/// <summary>
		/// Creates and adds link entities which point to
		/// linkedEntities, without modifying their hierarchy.
		/// </summary>
		public static void AddLinks(this Entity parent, IEnumerable<Entity> linkedEntities)
		{
			if (linkedEntities == null)
				throw new ArgumentNullException("linkedEntities");

			parent.Add(linkedEntities.Select(
				linkedEntity => CreateLinkEntity(parent, linkedEntity)));
		}

		/// <summary>
		/// Adds a (labeled) group with new link entities
		/// pointing to the specified linkedEntities.
		/// </summary>
		public static Entity AddLinkGroup(this Entity parent, string groupName, IEnumerable<Entity> linkedEntities)
		{
			var group = parent.AddGroup(groupName);
			group.AddLinks(linkedEntities);
			return group;
		}

		/// <summary>
		/// If the entity is a link entity, returns the linked
		/// entity, otherwise just returns the entity itself.
		/// </summary>
		public static Entity FollowLinked(this Entity entity)
		{
			return (entity.Has<HierarchyLinkComponent>()
				? entity.GetOrThrow<TargetComponent>().Value
				: entity);
		}
		/// <summary>
		/// If any entity in this enumerable is a link entity, yields
		/// the linked entity, otherwise just yields the entity itself.
		/// </summary>
		public static IEnumerable<Entity> FollowLinked(this IEnumerable<Entity> entities)
		{
			return entities.Select(entity => FollowLinked(entity));
		}

		#endregion
	}
}

