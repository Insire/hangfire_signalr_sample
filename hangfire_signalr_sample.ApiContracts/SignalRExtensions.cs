using Microsoft.AspNetCore.SignalR.Client;

namespace hangfire_signalr_sample.ApiContracts
{
    public sealed class SignalRExtensions
    {
        public static HubConnection GetHubConnection(string url)
        {
            var uri = new Uri(url + "/jobs");
            return new HubConnectionBuilder()
                .WithUrl(uri)
                .Build();
        }
    }
}