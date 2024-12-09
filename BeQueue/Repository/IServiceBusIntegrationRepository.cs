using Azure.Messaging.ServiceBus;

namespace BeQueue.Repository;

public interface IServiceBusIntegrationRepository
{
  Task<IReadOnlyList<ServiceBusReceivedMessage>> GetMessageAsync(string queueName);
  Task<bool> GetAdministratorClient(string queueName);
  Task SendMessageAsync(string queueName, ServiceBusMessage message);
}