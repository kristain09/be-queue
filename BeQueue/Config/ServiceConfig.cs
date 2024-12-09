using BeQueue.Repository;
using BeQueue.Service;

namespace BeQueue.Config;

public static class ServiceConfig
{
  public static void InjectService(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddScoped<IJobService, JobService>();
    serviceCollection.AddSingleton<IJobPublisherRepository, JobPublisherRepositoryRepository>();
    serviceCollection.AddSingleton<IJobConsumerRepository, JobConsumerRepository>();
    serviceCollection.AddSingleton<IServiceBusIntegrationRepository, ServiceBusIntegrationRepository>();
  }
}