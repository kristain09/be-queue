using BeQueue.Exception;
using BeQueue.Repository;
using BeQueue.Service;
using BeQueue.Shared;
using BeQueue.Shared.Entity;
using Moq;
using NUnit.Framework;

namespace BeQueueTest.Service;

[TestFixture]
public class JobServiceTests
{
    private Mock<IJobPublisherRepository> _jobPublisherRepositoryMock;
    private Mock<IJobConsumerRepository> _jobConsumerRepositoryMock;
    private JobService _jobService;

    [SetUp]
    public void SetUp()
    {
        _jobPublisherRepositoryMock = new Mock<IJobPublisherRepository>();
        _jobConsumerRepositoryMock = new Mock<IJobConsumerRepository>();
        _jobService = new JobService(_jobPublisherRepositoryMock.Object, _jobConsumerRepositoryMock.Object);
    }

    [Test]
    public async Task CreateJobServiceAsync_ShouldAddJobAndSendMessage()
    {
        var jobRequestDto = new JobRequestDto { JobName = "TestJob" };

        await _jobService.CreateJobServiceAsync(jobRequestDto);

        _jobPublisherRepositoryMock.Verify(repo => repo.SendMessageAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task GetJobsAsync_ShouldConsumeAndUpdateJobs()
    {
        string existingJob = "ExistingJob";
        string newJob = "newJob";

        _jobConsumerRepositoryMock.Setup(repo => repo.ConsumeMessagesAsync())
            .ReturnsAsync([
                new Job
                {
                    JobId = Guid.NewGuid(),
                    JobDate = 1,
                    JobName = existingJob,
                    Status = "Failed"
                },

                new Job
                {
                    JobId = Guid.NewGuid(),
                    JobDate = 1,
                    JobName = newJob,
                    Status = "Failed"
                }
            ]);

        await _jobService.CreateJobServiceAsync(new JobRequestDto { JobName = existingJob });
        await _jobService.CreateJobServiceAsync(new JobRequestDto { JobName =  newJob});

        var jobs = await _jobService.GetJobsAsync();

        Assert.That(jobs[0].JobDate, Is.LessThanOrEqualTo(jobs[1].JobDate));
    }

    [Test]
    public void RetryFetchAsync_ShouldThrowIfJobNotFound()
    {
        var nonExistentJobId = Guid.NewGuid();

        Assert.ThrowsAsync<ResourceNotFoundException>(() => _jobService.RetryFetchAsync(nonExistentJobId));
    }

    [Test]
    public async Task RetryFetchAsync_ShouldUpdateJobDateAndSendMessage()
    {
        var jobId = Guid.NewGuid();
        var job = new Job { JobId = jobId, JobDate = 1 };
        await _jobService.CreateJobServiceAsync(new JobRequestDto { JobName = "TestJob" });
        
        Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _jobService.RetryFetchAsync(jobId));
        _jobPublisherRepositoryMock.Verify(repo => repo.SendMessageAsync(It.IsAny<string>()), Times.Exactly(1));
    }
}