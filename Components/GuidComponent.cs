using System;

namespace OtherEngine.Core.Components
{
	public class GuidComponent : SimpleComponent<Guid>
	{
		public GuidComponent(Guid guid) : base(guid) {  }
	}
}

