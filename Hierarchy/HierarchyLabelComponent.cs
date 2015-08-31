using OtherEngine.Core.Components;

namespace OtherEngine.Core.Hierarchy
{
	/// <summary>
	/// Added to entities when they're added to the
	/// hierarchy of another entity using a label.
	/// </summary>
	public class HierarchyLabelComponent : SimpleComponent<string>
	{
		internal HierarchyLabelComponent(string label)
			: base(label) {  }
	}
}

