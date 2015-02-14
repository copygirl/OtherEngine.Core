using System;

namespace OtherEngine.Core.Events
{
	public interface IEventListener<T> where T : IEvent
	{
		void Receive(T ev);
	}
}

