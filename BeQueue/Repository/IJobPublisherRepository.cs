namespace BeQueue.Repository;

public interface IJobPublisherRepository
{
  Task SendMessageAsync(string message);
}