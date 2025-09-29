using Temporalio.Client;
using MyNamespace;

var address = Environment.GetEnvironmentVariable("TEMPORAL_ADDRESS") ?? "temporal:7233";
var ns      = Environment.GetEnvironmentVariable("TEMPORAL_NAMESPACE") ?? "default";
var tq      = Environment.GetEnvironmentVariable("TASK_QUEUE") ?? "my-task-queue";

Console.WriteLine($"[starter] Conectando ao Temporal em '{address}' (ns='{ns}', tq='{tq}')");

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
        Console.WriteLine($"Temporal ainda indisponível (tentativa {i}): {ex.Message}");
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

if (client is null)
{
    throw new InvalidOperationException(
        $"Não foi possível resolver/conectar ao Temporal em '{address}' após 12 tentativas.");
}

var result = await client.ExecuteWorkflowAsync(
    (SayHelloWorkflow wf) => wf.RunAsync("Temporal"),
    new(id: "my-workflow-id", taskQueue: tq));

Console.WriteLine($"Workflow result: {result}");
