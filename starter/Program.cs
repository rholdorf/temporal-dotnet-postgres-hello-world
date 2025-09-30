using Microsoft.Extensions.Logging;
using Temporalio.Client;
using MyNamespace;

using var loggerFactory = LoggerFactory.Create(builder => 
{
	builder.AddSimpleConsole(opt => 
	{
		opt.IncludeScopes = true;
		opt.SingleLine = true;
		opt.TimestampFormat = "HH:mm:ss ";
	})
	.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger("starter");

var address = Environment.GetEnvironmentVariable("TEMPORAL_ADDRESS") ?? "temporal:7233";
var ns      = Environment.GetEnvironmentVariable("TEMPORAL_NAMESPACE") ?? "default";
var tq      = Environment.GetEnvironmentVariable("TASK_QUEUE") ?? "my-task-queue";

logger.LogInformation($"[starter] Conectando ao Temporal em '{address}' (ns='{ns}', tq='{tq}')");

TemporalClient? client = null;
for (var i = 1; i <= 12; i++)
{
    try
    {
        client = await TemporalClient.ConnectAsync(new(address) { Namespace = ns });
        break;
    }
    catch (Exception ex)
    {
        logger.LogWarning($"Temporal ainda indisponível (tentativa {i}): {ex.Message}");
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

if (client is null)
{
    throw new InvalidOperationException(
        $"Não foi possível resolver/conectar ao Temporal em '{address}' após 12 tentativas.");
}

logger.LogInformation("Iniciando workflow...");

var result = await client.ExecuteWorkflowAsync(
    (SayHelloWorkflow wf) => wf.RunAsync("Temporal"),
    new(id: "my-workflow-id", taskQueue: tq));

logger.LogInformation($"Workflow result: {result}");
