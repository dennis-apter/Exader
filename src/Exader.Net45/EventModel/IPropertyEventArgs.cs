using System.ComponentModel;

namespace Exader.EventModel
{
	public interface IPropertyEventArgs : IEventArgs
	{
		PropertyChangedEventArgs Property { get; }
	}
}