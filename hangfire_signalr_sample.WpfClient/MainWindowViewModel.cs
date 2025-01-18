using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace hangfire_signalr_sample.WpfClient
{
    public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
    {
        private readonly SourceCache<MessageViewModel, Guid> _items;
        private readonly ReadOnlyObservableCollection<MessageViewModel> _readonlyItems;
        private readonly CancellationTokenSource _cts;
        private readonly HubConnection _hubConnection;
        private readonly IDisposable _subscription;

        public ReadOnlyObservableCollection<MessageViewModel> Items => _readonlyItems;

        public MainWindowViewModel(HubConnection hubConnection, IScheduler scheduler)
        {
            _cts = new CancellationTokenSource();
            _items = new SourceCache<MessageViewModel, Guid>(p => p.Id);
            _hubConnection = hubConnection;

            var sub1 = _items
                 .Connect()
                 .ObserveOn(scheduler)
                 .Bind(out _readonlyItems)
                 .Subscribe();

            var sub2 = _hubConnection.On<Guid, string, string>("ReceiveMessage", (messageId, user, message) =>
            {
                _items.AddOrUpdate(new MessageViewModel(messageId, user, message));
            });

            _subscription = new CompositeDisposable(sub1, sub2);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        private async Task Connect()
        {
            await _hubConnection.StartAsync(_cts.Token);

            await _hubConnection.SendAsync("SendMessage", Guid.NewGuid(), "WpfClient", "Hello from WpfClient!");
        }

        public async void Dispose()
        {
            await _cts.CancelAsync();

            _subscription.Dispose();
            _cts.Dispose();
        }
    }
}