using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BeQueue.Shared.Entity;

public class Job
{
  public Guid JobId { get; set; }
  [Required]
  public string JobName { get; set; }

  public string Status { get; set; } = "On Process";
  public long JobDate { get; set; }
  
  [SetsRequiredMembers]
  public Job(JobRequestDto requestDto)
  {
    JobId = Guid.NewGuid();
    JobName = requestDto.JobName;
    JobDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  }

  public Job()
  {
  }
}