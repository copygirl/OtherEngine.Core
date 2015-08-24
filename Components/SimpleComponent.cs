
namespace OtherEngine.Core.Components
{
	/// <summary>
	/// Base class for single value components.
	/// The value setter in this class is protected.
	/// </summary>
	public abstract class SimpleComponent<TValue> : Component
	{
		public TValue Value { get; protected set; }

		protected SimpleComponent() {  }
		protected SimpleComponent(TValue value) { Value = value; }
	}

	/// <summary>
	/// Base class for single value components.
	/// The value setter in this class is public.
	/// </summary>
	public abstract class SimplePublicComponent<TValue> : SimpleComponent<TValue>
	{
		public new TValue Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
	}
}

