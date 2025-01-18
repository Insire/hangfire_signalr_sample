using CommunityToolkit.Mvvm.ComponentModel;

namespace hangfire_signalr_sample.WpfClient
{
    public sealed class MessageViewModel : ObservableObject
    {
        public Guid Id { get; }
        public string User { get; }
        public string Message { get; }

        public MessageViewModel(Guid id, string user, string message)
        {
            Id = id;
            User = user;
            Message = message;
        }
    }
}