using System;

namespace OtherEngine.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class IdentifierAttribute : Attribute
	{
		public string Identifier { get; private set; }


		public IdentifierAttribute(string identifier)
		{
			if (identifier == null)
				throw new ArgumentNullException("identifier");
			
			Identifier = identifier;
		}
	}
}

