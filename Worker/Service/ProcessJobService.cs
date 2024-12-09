using System.Text.Json;
using Azure.Messaging.ServiceBus;
using BeQueue.Shared.Entity;

namespace Worker.Service;

public class ProcessJobService
{
  private List<Job> Jobs { get; set; } = [];
  private static readonly  string MessageBusConnection = Environment.GetEnvironmentVariable("MESSAGE_BUS_CONNECTION") 
                                                          ?? throw new InvalidOperationException("MESSAGE_BUS_CONNECTION environment variable is not set.");
  private const string RetrieveJobQueue = "submit-job";
  private const string SendJobQueue = "processed-job";
  private readonly ServiceBusClient _serviceBusClient = new(MessageBusConnection);

  private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
  {
      IncludeFields = true,
      PropertyNameCaseInsensitive = true
  };
  
  public async Task StartProcessing(CancellationToken cancellationToken)
    {
        var processor = _serviceBusClient.CreateProcessor(RetrieveJobQueue);

        processor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();
            var job = JsonSerializer.Deserialize<Job>(body, _jsonSerializerOptions);
            if (job != null)
            {
                await ProcessJob(job);
            }
            await args.CompleteMessageAsync(args.Message, cancellationToken);
        };

        processor.ProcessErrorAsync += args => Task.CompletedTask;

        await processor.StartProcessingAsync(cancellationToken);
        
        Console.WriteLine("Press any key to terminate worker");
        Console.ReadKey();

        await processor.StopProcessingAsync(cancellationToken);
    }

    private async Task ProcessJob(Job job)
    {
        var jobWithSameName = Jobs.Where(j => j.JobName == job.JobName).ToList();
        if (jobWithSameName.Count >= 1 && !jobWithSameName.Exists(j => j.JobId == job.JobId))
        {
            job.Status = "Rejected";
            Jobs.Add(job);
        }
        else
        {
            job.Status = "Processed";
            Jobs.Add(job);

            await Task.Delay(5000);
            var random = new Random();
            job.Status = random.Next(1, 101) <= 50 ? "Success" : "Failed";
        }

        await SendProcessedJob(job);
    }

    private async Task SendProcessedJob(Job job)
    {
        var sender = _serviceBusClient.CreateSender(SendJobQueue);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(job))
        {
            ContentType = "application/json"
        };
        await sender.SendMessageAsync(message);
    }
}
