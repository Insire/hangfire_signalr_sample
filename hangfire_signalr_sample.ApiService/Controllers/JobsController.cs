using Hangfire;
using hangfire_signalr_sample.ApiContracts;
using hangfire_signalr_sample.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hangfire_signalr_sample.ApiService.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class JobsController : ControllerBase
{
    private readonly IBackgroundJobClientV2 _backgroundJobClient;
    private readonly HubConnection _hubConnection;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IBackgroundJobClientV2 backgroundJobClient, HubConnection hubConnection, ILogger<JobsController> logger)
    {
        _backgroundJobClient = backgroundJobClient;
        _hubConnection = hubConnection;
        _logger = logger;
    }

    // POST api/<JobsController>
    [HttpPost]
    public async Task Post([FromBody] JobDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Begin Post: {0}", dto.Value);

        try
        {
            if (!(_hubConnection.State == HubConnectionState.Connected || _hubConnection.State == HubConnectionState.Connecting))
            {
                await _hubConnection.StartAsync();
            }

            var tcs = new TaskCompletionSource();
            using var registration = cancellationToken.Register(() => tcs.SetCanceled());

            var jobId = string.Empty;
            using var subscription = _hubConnection.On<Guid, string, string>("ReceiveMessage", (messageId, user, message) =>
            {
                _logger.LogInformation("Post ReceiveMessage: {0} {1} {2}", messageId, user, message);
                if (message == jobId)
                {
                    _logger.LogInformation("Post ReceiveMessage accepted: {0} {1} {2}", messageId, user, message);
                    tcs.SetResult();
                }
            });

            _logger.LogInformation("Post queue jobs: {0}", dto.Value);

            var value = dto.Value;
            jobId = _backgroundJobClient.Enqueue<JobsService>((service) => service.Job(value));
            _backgroundJobClient.ContinueJobWith<JobsService>(jobId, (service) => service.PostJob(jobId));

            _logger.LogInformation("Post await callback: {0}", dto.Value);

            await tcs.Task;
        }
        finally
        {
            _logger.LogInformation("End Post: {0}", dto.Value);
        }
    }
}