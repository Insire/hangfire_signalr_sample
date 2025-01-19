namespace hangfire_signalr_sample.ApiContracts
{
    public interface IJobClient
    {
        Task ReceiveMessage(Guid messageId, string user, string message);
    }
}