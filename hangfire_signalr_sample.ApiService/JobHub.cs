using Microsoft.AspNetCore.SignalR;

namespace hangfire_signalr_sample.ApiService;

public sealed class JobHub(ILogger<Hub>logger) : Hub
{
    readonly ILogger<Hub> _logger = logger;

    public async Task SendMessage(Guid messageId, string user, string message)
    {
        _logger.LogDebug("ReceiveMessage {Id} {Content} from {User}",messageId,message, user);
        await Clients.All.SendAsync("ReceiveMessage",messageId, user, message);
    }
}