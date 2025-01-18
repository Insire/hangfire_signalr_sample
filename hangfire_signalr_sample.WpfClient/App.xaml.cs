using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Windows;

namespace hangfire_signalr_sample.WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private readonly IConfigurationRoot _configuration;
        private readonly HubConnection _hubConnection;

        public App()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var url = _configuration.GetValue<string>("services:apiservice:https:0");
            var uri = new Uri(url + "/jobs");

            _serviceProvider = new ServiceCollection()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<IConfiguration>(_configuration)
                .AddSingleton(new HubConnectionBuilder()
                    .WithUrl(uri)
                    .Build())
                .AddSingleton<IScheduler>(_ =>
                {
                    return Dispatcher.Invoke(() => new SynchronizationContextScheduler(SynchronizationContext.Current!));
                })
                .BuildServiceProvider();
        }

        override protected void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow()
            {
                DataContext = viewModel,
            };

            viewModel.ConnectCommand.Execute(null);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();
            base.OnExit(e);
        }
    }
}