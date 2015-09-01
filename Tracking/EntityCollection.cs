using System.Collections.Generic;
using System.Linq;

namespace OtherEngine.Core.Tracking
{
	public abstract class EntityCollection
		: IReadOnlyCollection<Entity>
	{
		public abstract int Count { get; }


		internal abstract void Add(Entity entity);

		internal abstract void Remove(Entity entity);


		#region IEnumerable implementation

		protected abstract IEnumerator<Entity> GetEnumeratorInternal();

		IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
		{
			return GetEnumeratorInternal();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumeratorInternal();
		}

		#endregion
	}

	public class EntityCollection<TComponent>
		: EntityCollection, IReadOnlyCollection<EntityRef<TComponent>>
		where TComponent : Component
	{
		HashSet<EntityRef<TComponent>> _tracked = new HashSet<EntityRef<TComponent>>();

		public override int Count { get { return _tracked.Count; } }


		internal EntityCollection() {  }


		#region IEntityCollection implementation

		internal override void Add(Entity entity) { _tracked.Add(entity); }

		internal override void Remove(Entity entity) { _tracked.Remove(entity); }

		#endregion

		#region IEnumerable implementation

		public IEnumerator<EntityRef<TComponent>> GetEnumerator()
		{
			return _tracked.GetEnumerator();
		}

		protected override IEnumerator<Entity> GetEnumeratorInternal()
		{
			return ((IEnumerable<EntityRef<TComponent>>)this).Select(r => r.Entity).GetEnumerator();
		}

		#endregion
	}
}

