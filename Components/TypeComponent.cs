using OtherEngine.Core.Attributes;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Holds the entity type name, used to describe the nature of an entity.
	/// Don't use this for anything other than displaying debug information.
	/// This is used in Entity.ToString.
	/// 
	/// For example: Controller, World, Texture, Material, Player, Item, ...
	/// </summary>
	[DefaultComponentValue("Entity")]
	public class TypeComponent : SimplePublicComponent<string>
	{
	}
}

