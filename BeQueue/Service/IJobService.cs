using BeQueue.Shared;
using BeQueue.Shared.Entity;

namespace BeQueue.Service;

public interface IJobService
{
  Task CreateJobServiceAsync(JobRequestDto request);
  Task<List<Job>> GetJobsAsync();
  Task RetryFetchAsync(Guid jobId);
}