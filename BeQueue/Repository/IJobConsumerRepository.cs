using BeQueue.Shared.Entity;

namespace BeQueue.Repository;

public interface IJobConsumerRepository
{
  Task<List<Job>> ConsumeMessagesAsync();
}