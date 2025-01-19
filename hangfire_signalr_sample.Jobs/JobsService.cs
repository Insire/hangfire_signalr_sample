using Hangfire;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace hangfire_signalr_sample.Jobs;

public sealed class JobsService
{
    private readonly ILogger<JobsService> _logger;
    private readonly HubConnection _hubConnection;

    public JobsService(ILogger<JobsService> logger, HubConnection hubConnection)
    {
        _logger = logger;
        _hubConnection = hubConnection;
    }

    [Queue("default")]
    public void Job(string value)
    {
        _logger.LogInformation("Job: {0}", value);
    }

    [Queue("default")]
    public async Task PostJob(string jobId)
    {
        _logger.LogInformation("PostJob: {0}", jobId);

        if (!(_hubConnection.State == HubConnectionState.Connected || _hubConnection.State == HubConnectionState.Connecting))
        {
            await _hubConnection.StartAsync();
        }

        await _hubConnection.SendAsync("SendMessage", Guid.NewGuid(), "WorkerService", jobId);
    }
}