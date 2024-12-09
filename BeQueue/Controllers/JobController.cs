using System.ComponentModel.DataAnnotations;
using BeQueue.Service;
using BeQueue.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BeQueue.Controllers;

[ApiController]
[Route("api/job")]
public class JobController(IJobService jobService)
  : ControllerBase
{
  [SwaggerOperation(Summary = "Create a new Job", Description = "Create Job needs at least 3 character")]
  [HttpPost]
  public async Task<IActionResult> CreateJobAsync([Required] JobRequestDto jobRequestDto)
  {
    await jobService.CreateJobServiceAsync(jobRequestDto);
    return Ok();
  }

  [HttpGet]
  [SwaggerOperation("Get Job", Description = "This API will response with list of Job ordered by Date and Time\n" +
                                             "Status of jobs: " +
                                             "- Failed, -On Process -Rejected -Success")]
  public async Task<IActionResult> GetJobsAsync()
  {
    return Ok(await jobService.GetJobsAsync());
  }

  [HttpPost("{id:guid}")]
  [SwaggerOperation("Operation to retry a job", "This API is used for retry ONLY failed job")]
  public async Task<IActionResult> RetryFailedJob([Required] Guid id)
  {
    await jobService.RetryFetchAsync(id);
    return Ok();
  }
}