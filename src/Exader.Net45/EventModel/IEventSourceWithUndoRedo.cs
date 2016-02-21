namespace Exader.EventModel
{
	public interface IEventSourceWithUndoRedo : IEventSource
	{
		void Redo();

		void Undo();
		bool IsProposed { get; }
	}
}