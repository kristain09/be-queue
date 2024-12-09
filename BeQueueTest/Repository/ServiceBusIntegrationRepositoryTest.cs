using BeQueue.Repository;
using NUnit.Framework;

namespace BeQueueTest.Repository;

[TestFixture]
public class ServiceBusIntegrationRepositoryTest
{
  [Test]
  public async Task IntegrationTestConsumeMessageAsync_QueueShallExists_QueueNameIsExists()
  {
    var serviceBusClient = new ServiceBusIntegrationRepository();
    
    var queueIsExists = await serviceBusClient.GetAdministratorClient("processed-job");
    
    Assert.That(queueIsExists, Is.True);
  }
  
  [Test]
  public async Task IntegrationTestConsumeMessageAsync_QueueShallExists_QueueNameIsNotExists()
  {
    var serviceBusClient = new ServiceBusIntegrationRepository();
    
    var queueIsExists = await serviceBusClient.GetAdministratorClient("not-exists");
    
    Assert.That(queueIsExists, Is.False);
  }
}