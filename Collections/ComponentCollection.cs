namespace OtherEngine.Core.Collections
{
	public class ComponentCollection : TypedCollection<IGameComponent>
	{
		public GameEntity Entity { get; private set; }

		internal ComponentCollection(GameEntity entity)
		{
			Entity = entity;
		}

		protected override void OnAdded(IGameComponent component)
		{
			Entity.Game.Components.OnAdded(Entity, component);
		}
		protected override void OnRemoved(IGameComponent component)
		{
			Entity.Game.Components.OnRemoved(Entity, component);
		}
	}
}

