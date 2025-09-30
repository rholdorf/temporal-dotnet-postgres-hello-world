namespace Worker;

using Temporalio.Activities;
using Microsoft.Extensions.Logging;

public class MyActivities(ILogger<MyActivities> logger)
{
    private readonly ILogger<MyActivities> _logger = logger;

    // Activities can be async and/or static too! We just demonstrate instance
    // methods since many will use them that way.
    [Activity]
    public string SayHello(string name)
    {
        _logger.LogInformation("activity was invoked");
        return $"Hello, {name}!";
    }
}