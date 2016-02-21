using System.Collections.Generic;
using System.ComponentModel;

namespace Exader.EventModel
{
	public interface IEventSource : IRevertibleChangeTracking
	{
		void DispatchEvent(object sender, IEventArgs args);

		void LoadEvent(IEventArgs args);

		/// <summary>
		/// Формирует извещения о новом событии и применяет его.
		/// </summary>
		/// <param name="args"></param>
		void RaiseEvent(IEventArgs args);

		IEnumerable<IEventArgs> PendingEvents { get; }
	}
}
