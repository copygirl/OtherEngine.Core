using System;

namespace OtherEngine.Core.Exceptions
{
	/// <summary>
	/// Thrown when there was an error when validating or handling an attribute.
	/// </summary>
	public abstract class AttributeException : Exception
	{
		public Attribute Attribute { get; private set; }

		/// <summary>
		/// Gets the entity the attribute is associated with.
		/// </summary>
		public object Associated { get; private set; }

		protected AttributeException(Attribute attribute, object associated, string message, Exception innerException)
			: base(message ?? GetDefaultMessage(attribute, associated, innerException), innerException)
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");
			if (associated == null)
				throw new ArgumentNullException("associated");
			
			Attribute = attribute;
			Associated = associated;
		}

		static string GetDefaultMessage(Attribute attribute, object associated, Exception innerException)
		{
			return string.Format("Exception when handling attribute {0} of {1}{2}",
				attribute, associated, ((innerException != null) ? (": " + innerException.Message) : ""));
		}
	}

	/// <summary>
	/// Thrown when there was an error when validating or handling an attribute.
	/// </summary>
	public class AttributeException<TEntity> : AttributeException
	{
		/// <summary>
		/// Gets the entity the attribute is associated with.
		/// </summary>
		public new TEntity Associated { get { return (TEntity)base.Associated; } }

		public AttributeException(Attribute attribute, TEntity associated, string message = null, Exception innerException = null)
			: base(attribute, associated, message, innerException) {  }
	}
}

