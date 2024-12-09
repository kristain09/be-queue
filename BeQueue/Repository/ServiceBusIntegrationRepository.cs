using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using BeQueue.Exception;

namespace BeQueue.Repository;

public class ServiceBusIntegrationRepository() : IServiceBusIntegrationRepository
{
  private static readonly string ServiceBusConnectionString =
    Environment.GetEnvironmentVariable("MESSAGE_BUS_CONNECTION")
    ?? throw new InvalidOperationException("MESSAGE_BUS_CONNECTION environment variable is not set.");
  
  private static ServiceBusClient GetServiceBusClient()
  {
    return new ServiceBusClient(ServiceBusConnectionString);
  }

  public async Task<bool> GetAdministratorClient(string queueName)
  {
    var serviceBusAdministrationClient = new ServiceBusAdministrationClient(ServiceBusConnectionString);
    var queueExistsAsync = await serviceBusAdministrationClient.QueueExistsAsync(queueName);
    return queueExistsAsync.Value;
  }

  public async Task<IReadOnlyList<ServiceBusReceivedMessage>> GetMessageAsync(string queueName)
  {
    var serviceBusClient = GetServiceBusClient();
    var receiver = serviceBusClient.CreateReceiver(queueName);
    try
    {

      var waitTime = TimeSpan.FromMilliseconds(2000);
      var message = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: waitTime);
      foreach (var serviceBusReceivedMessage in message)
      {
        await receiver.CompleteMessageAsync(serviceBusReceivedMessage);
      }
      return message;
    }
    catch
    {
      throw new AzureNotAvailable("message bus not available");
    }
  }
  
  public async Task SendMessageAsync(string queueName, ServiceBusMessage message)
  {
    var serviceBusClient = GetServiceBusClient();
    var sender = serviceBusClient.CreateSender(queueName);
    try
    {
      await sender.SendMessageAsync(message);
    }
    catch
    {
      throw new AzureNotAvailable("message bus not available");
    }
    finally
    {
      await sender.DisposeAsync();
    }
  }
}