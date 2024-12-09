using System.Text.Json;
using BeQueue.Exception;
using BeQueue.Shared.Entity;

namespace BeQueue.Repository;

public class JobConsumerRepository(IServiceBusIntegrationRepository serviceBus, IConfiguration configuration)
  : IJobConsumerRepository
{
  private readonly string _queueName = configuration.GetValue<string>("AzureServiceBus:QueueNameReader")!;

  public async Task<List<Job>> ConsumeMessagesAsync()
  {
    var receivedMessages = await serviceBus.GetMessageAsync(_queueName);

    return receivedMessages.Select(serviceBusReceivedMessage => serviceBusReceivedMessage.Body.ToString()).Select(data => JsonSerializer.Deserialize<Job>(data)!).ToList();
  }
}