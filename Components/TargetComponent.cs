
namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Holds the target for an entity.
	/// This can be used in multiple different contexts.
	/// 
	/// If there's need for multiple target components on a single entity
	/// (which isn't possible), either create a more specific component or
	/// use multiple sub-entities.
	/// </summary>
	public class TargetComponent : SimplePublicComponent<Entity>
	{
	}
}

