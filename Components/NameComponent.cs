using System;
using OtherEngine.ES;

namespace OtherEngine.Core.Components
{
	public struct NameComponent : IComponent, IEquatable<NameComponent>
	{
		public string Name { get; private set; }

		public NameComponent(string name) { Name = name; }


		#region ToString, Equals and GetHashCode

		public override string ToString()
		{
			return string.Format("[Name \"{0}\"]", Name);
		}

		public override bool Equals(object obj)
		{
			return ((obj is NameComponent) && Equals((NameComponent)obj));
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		#region IEquatable<NameComponent> implementation

		public bool Equals(NameComponent other)
		{
			return (Name == other.Name);
		}

		#endregion
	}
}

