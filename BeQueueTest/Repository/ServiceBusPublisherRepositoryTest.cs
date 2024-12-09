using Azure.Messaging.ServiceBus;
using BeQueue.Repository;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace BeQueueTest.Repository;

public class ServiceBusPublisherRepositoryTest
{
  private Mock<IConfiguration> _config;

  [SetUp]
  public void Setup()
  {
    const string queue = "queue-name";
    var configurationSection = new Mock<IConfigurationSection>(MockBehavior.Loose);
    configurationSection.Setup(x => x.Value)
      .Returns(() => queue);

    _config = new Mock<IConfiguration>();
    _config.Setup(c => c.GetSection("AzureServiceBus:QueueName"))
      .Returns(configurationSection.Object);
  }

  [Test]
  public async Task ConsumeMessageAsync_ShallReturnEmptyList_NoJobInQueue()
  {
    var serviceBusRepoMock = new Mock<IServiceBusIntegrationRepository>();
    var publisher = new JobPublisherRepositoryRepository(serviceBusRepoMock.Object, _config.Object);
    await publisher.SendMessageAsync("a string");
    serviceBusRepoMock.Verify(repo => repo.SendMessageAsync(It.IsAny<string>(), It.IsAny<ServiceBusMessage>()),
      Times.Once);
  }
}