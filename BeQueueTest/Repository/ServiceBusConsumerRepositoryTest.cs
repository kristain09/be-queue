using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BeQueue.Repository;
using BeQueue.Shared.Entity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using IConfigurationSection = Microsoft.Extensions.Configuration.IConfigurationSection;

namespace BeQueueTest.Repository;

public class ServiceBusConsumerRepositoryTest
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
    _config.Setup(c => c.GetSection("AzureServiceBus:QueueNameReader"))
      .Returns(configurationSection.Object);
  }

  [Test]
  public async Task ConsumeMessageAsync_ShallReturnEmptyList_NoJobInQueue()
  {
    var serviceBusRepoMock = new Mock<IServiceBusIntegrationRepository>();
    var resultMessage = new List<ServiceBusReceivedMessage>().AsReadOnly();
    serviceBusRepoMock.Setup(serviceBus =>
        serviceBus.GetMessageAsync(_config.Object.GetValue<string>("AzureServiceBus:QueueNameReader")!))
      .ReturnsAsync(() => resultMessage);
    var jobConsumerRepository = new JobConsumerRepository(serviceBusRepoMock.Object, _config.Object);
    
    var consumeMessagesAsync = await jobConsumerRepository.ConsumeMessagesAsync();
    
    Assert.That(consumeMessagesAsync, Is.Empty);
  }
  
  [Test]
  public async Task ConsumeMessageAsync_ShallReturnEmptyList_OneJobInQueue()
  {
    var serviceBusRepoMock = new Mock<IServiceBusIntegrationRepository>();
    var job = new Job()
    {
      JobId = new Guid("27d308f9-9edd-4b16-96b7-274316ec2dad"),
      JobDate = 1733669470855,
      JobName = "Job 1",
      Status = "On Process"
    };
    var data = JsonSerializer.Serialize(job);
    var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(body: new BinaryData(data));
    var serviceBusReceivedMessages = new List<ServiceBusReceivedMessage>{serviceBusReceivedMessage};
    serviceBusRepoMock.Setup(serviceBus =>
        serviceBus.GetMessageAsync(_config.Object.GetValue<string>("AzureServiceBus:QueueNameReader")!))
      .ReturnsAsync(() => serviceBusReceivedMessages.AsReadOnly());
    
    var jobConsumerRepository = new JobConsumerRepository(serviceBusRepoMock.Object, _config.Object);
    var consumeMessagesAsync = await jobConsumerRepository.ConsumeMessagesAsync();
    
    Assert.That(consumeMessagesAsync.Count, Is.EqualTo(1));
  }
}