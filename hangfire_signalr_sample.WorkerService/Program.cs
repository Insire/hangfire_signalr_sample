using Hangfire;
using hangfire_signalr_sample.Jobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace hangfire_signalr_sample.WorkerService;

internal sealed class Program
{
    static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        IHost host = null!;

        builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseActivator(new HangfireActivator(() => host))
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("database"), new Hangfire.SqlServer.SqlServerStorageOptions()
            {
                SqlClientFactory = SqlClientFactory.Instance,
                TryAutoDetectSchemaDependentOptions = false,
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true, // Migration to Schema 7 is required
                PrepareSchemaIfNecessary = false,
                SchemaName = "hangfire",
            }));

        builder.Services.AddHangfireServer(configuration =>
        {
            configuration.WorkerCount = 1;
            configuration.Queues = ["default"];
        });

        builder.Services.AddSingleton<JobsService>();

        host = builder.Build();
        host.Run();
    }
}