using System;
using OtherEngine.Core.Collections;

namespace OtherEngine.Core
{
	public sealed class Entity
	{
		public ComponentCollection Components { get; private set; }

		public Entity()
		{
			Components = new ComponentCollection(this);
		}
	}
}

