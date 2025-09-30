using Microsoft.Extensions.Logging;
using Temporalio.Client;
using Temporalio.Worker;
using MyNamespace;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddSimpleConsole(opt =>
        {
            opt.IncludeScopes = true;
            opt.SingleLine = true;
            opt.TimestampFormat = "HH:mm:ss ";
        })
        .SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger("Worker");

// Create a client to localhost on "default" namespace
var client = await TemporalClient.ConnectAsync(
    new("temporal:7233")
    {
        Namespace = "default",
        LoggerFactory = loggerFactory
    });

// Cancellation token to shutdown worker on ctrl+c
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// Create worker with the activity and workflow registered
var activities = new MyActivities(loggerFactory.CreateLogger<MyActivities>());
var options = new TemporalWorkerOptions("my-task-queue")
   .AddAllActivities(activities)
   .AddWorkflow<SayHelloWorkflow>();

options.LoggerFactory = loggerFactory;

using var worker = new TemporalWorker(client, options);

logger.LogInformation("Worker started and waiting for tasks at task queue 'my-task-queue'");

// Run worker until cancelled
try
{
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    logger.LogInformation("Worker cancelled");
}
