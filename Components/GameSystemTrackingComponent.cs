using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Data;
using OtherEngine.Core.Systems;

namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Owned by <see cref="ComponentTrackerSystem"/>.
	/// 
	/// Component attached to GameSystem containers containing
	/// information regarding the components it's tracking.
	/// </summary>
	class GameSystemTrackingComponent : GameComponent
	{
		public TrackingCollection Tracking { get; set; }
	}

	class TrackingCollection : Dictionary<Type, PropertyInfo> {  }
}

