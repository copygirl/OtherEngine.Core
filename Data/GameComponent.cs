using OtherEngine.Core.Utility;
using System.Text.RegularExpressions;

namespace OtherEngine.Core.Data
{
	public abstract class GameComponent
	{
		static Regex _matchTrailingComponent = new Regex("Component$");

		/// <summary>
		/// Returns a friendly name of this component.
		/// For example, GuidComponent would return "Guid".
		/// </summary>
		public string Name { get { return _matchTrailingComponent.Replace(GetType().Name, ""); } }

		public override string ToString()
		{
			return string.Format("[Component: {0}]", Name);
		}
	}
}

