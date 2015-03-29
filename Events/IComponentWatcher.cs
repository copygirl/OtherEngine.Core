namespace OtherEngine.Core.Events
{
	public interface IComponentWatcher<TComponent> where TComponent : class, IGameComponent
	{
		void OnComponentAdded(GameEntity entity, TComponent component);

		void OnComponentRemoved(GameEntity entity, TComponent component);
	}
}

