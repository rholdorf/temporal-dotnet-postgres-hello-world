using Microsoft.Extensions.Logging;
using Temporalio.Client;
using Worker;

namespace Starter;

public static class Program
{
    private static readonly ILogger _logger;
    private static readonly ILoggerFactory? _loggerFactory;

    private static string? _serverAddress;
    private static string? _namespace;
    private static string? _taskQueue;

    static Program()
    {
        _loggerFactory = GetLoggerFactory();
        _logger = _loggerFactory.CreateLogger("starter");
    }

    public static async Task Main()
    {
        LoadSettings();

        _logger.LogInformation("Conectando ao Temporal em '{_serverAddress}' (ns='{_namespace}', tq='{_taskQueue}')", _serverAddress, _namespace, _taskQueue);

        TemporalClient? client = await TryGetClient();
        if (client == null)
        {
            _logger.LogError("Não foi possível conectar ao Temporal");
            return;
        }

        _logger.LogInformation("Iniciando workflow...");

        var result = await client.ExecuteWorkflowAsync(
            (SayHelloWorkflow wf) => wf.RunAsync("Temporal"),
            new(id: "my-workflow-id", taskQueue: _taskQueue!));

        _logger.LogInformation("Workflow result: {result}", result);
    }
    
    private static void LoadSettings()
    {
        _serverAddress = Environment.GetEnvironmentVariable("TEMPORAL_SERVER_ADDRESS") ?? "temporal:7233";
        _namespace = Environment.GetEnvironmentVariable("TEMPORAL_NAMESPACE") ?? "default";
        _taskQueue = Environment.GetEnvironmentVariable("TEMPORAL_TASK_QUEUE") ?? "my-task-queue";
    }

    private static async Task<TemporalClient?> TryGetClient()
    {
        TemporalClient? client = null;
        var attempts = 12;
        for (var i = 0; i < attempts; i++)
        {
            try
            {
                client = await TemporalClient.ConnectAsync(new(_serverAddress!) { Namespace = _namespace! });
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Temporal ainda indisponível (tentativa {i}/{attempts}): {ex.GetType().FullName} {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        return client;
    }

    private static ILoggerFactory GetLoggerFactory() =>
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(opt =>
            {
                opt.IncludeScopes = true;
                opt.SingleLine = true;
                opt.TimestampFormat = "HH:mm:ss ";
            })
            .SetMinimumLevel(LogLevel.Information);
        });
}