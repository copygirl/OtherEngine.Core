using System;
using System.Collections.Generic;

namespace OtherEngine.Core.Collections
{
	public class EntityCollection : EventedCollection<Entity>
	{
		private readonly HashSet<Entity> set = new HashSet<Entity>();

		#region implemented abstract members of EventedCollection

		public override int Count { get { return set.Count; } }

		protected override bool AddInternal(Entity entity)
		{
			if (!set.Add(entity))
				throw new ArgumentException(String.Format("Entity {0} is already in this {1}", entity, this), "entity");
			return true;
		}

		protected override bool ContainsInternal(Entity entity)
		{
			return set.Contains(entity);
		}

		protected override bool RemoveInternal(Entity entity)
		{
			if (!set.Remove(entity))
				throw new ArgumentException(String.Format("Entity {0} isn't in this {1}", entity, this), "entity");
		}

		protected override void ClearInternal()
		{
			set.Clear();
		}

		public override void CopyTo(Entity[] array, int arrayIndex)
		{
			set.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<Entity> GetEnumerator()
		{
			return set.GetEnumerator();
		}

		#endregion
	}
}

