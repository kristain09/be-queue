using System.Text.Json;
using BeQueue.Exception;
using BeQueue.Repository;
using BeQueue.Shared;
using BeQueue.Shared.Entity;

namespace BeQueue.Service;

public class JobService(IJobPublisherRepository jobPublisherRepository, IJobConsumerRepository jobConsumerRepository) : IJobService
{
  private static List<Job> _jobs = [];
  private readonly JsonSerializerOptions _jsonSerializerOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };
  public async Task CreateJobServiceAsync(JobRequestDto request)
  {
    var job = new Job(request);
    _jobs.Add(job);
    var message = JsonSerializer.Serialize(job, _jsonSerializerOptions);
    await jobPublisherRepository.SendMessageAsync(message);
  }

  public async Task<List<Job>> GetJobsAsync()
  {
    var busJobs = await jobConsumerRepository.ConsumeMessagesAsync();
    var busJobIds = busJobs.Select(job => job.JobId).ToHashSet();
    _jobs.RemoveAll(job => busJobIds.Contains(job.JobId));
    _jobs.AddRange(busJobs);
    return UpdateJobs();
  }

  public async Task RetryFetchAsync(Guid jobId)
  {
    var job = _jobs.FirstOrDefault(j => j.JobId == jobId);
    if (job == null)
      throw new ResourceNotFoundException("No job with specified id");
    job.JobDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    job.Status = "On Process";
    _jobs.Add(job);
    await jobPublisherRepository.SendMessageAsync(JsonSerializer.Serialize(job, _jsonSerializerOptions));
  }

  private List<Job> UpdateJobs()
  {
    var jobs = new List<Job>();
    jobs.AddRange(_jobs.DistinctBy(job => job.JobId).OrderBy(job => job.JobDate));
    return jobs;
  }
}