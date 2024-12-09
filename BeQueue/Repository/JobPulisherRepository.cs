using Azure.Messaging.ServiceBus;

namespace BeQueue.Repository;

public class JobPublisherRepositoryRepository(IServiceBusIntegrationRepository serviceBus, IConfiguration configuration) : IJobPublisherRepository
{
  private readonly string _queueName = configuration["AzureServiceBus:QueueName"]!;

  public async Task SendMessageAsync(string message)
  {
    await serviceBus.SendMessageAsync(_queueName, new ServiceBusMessage(message));
  }
}

