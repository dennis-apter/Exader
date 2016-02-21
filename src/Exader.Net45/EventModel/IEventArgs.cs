namespace Exader.EventModel
{
	public interface IEventArgs
	{
		void Apply(object target);

		void Decline(object target);
	}
}