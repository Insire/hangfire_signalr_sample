using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace hangfire_signalr_sample.WorkerService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
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
                configuration.Queues = new[] { "default" };
            });

            IHost host = builder.Build();
            host.Run();
        }
    }
}