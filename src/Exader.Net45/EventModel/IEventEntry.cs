namespace Exader.EventModel
{
	public interface IEventEntry
	{
		IEventArgs Args { get; }

		object Source { get; }
	}
}