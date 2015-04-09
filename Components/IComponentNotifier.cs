namespace OtherEngine.Core.Components
{
	public interface IComponentNotifier
	{
		void OnComponentAdded(GameEntity entity, IGameComponent component);

		void OnComponentRemoved(GameEntity entity, IGameComponent component);
	}
}

