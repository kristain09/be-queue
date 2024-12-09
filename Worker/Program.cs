using Worker.Service;

var processJobService = new ProcessJobService();
await processJobService.StartProcessing(new CancellationToken());