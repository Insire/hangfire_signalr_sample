using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace hangfire_signalr_sample.Bootstrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("database");

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions()
                {
                    SqlClientFactory = SqlClientFactory.Instance,
                    TryAutoDetectSchemaDependentOptions = false,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true, // Migration to Schema 7 is required
                    PrepareSchemaIfNecessary = false,
                    SchemaName = "hangfire"
                }));

            IHost host = builder.Build();

            var connectionStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.InitialCatalog = "master";

            using var connection = new SqlConnection(connectionStringBuilder.ToString());
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'DataBase')
                BEGIN
                    CREATE DATABASE [DataBase]
                END
            ";
            command.ExecuteNonQuery();

            command.CommandText = @"
            USE [DataBase]";
            command.ExecuteNonQuery();

            SqlServerObjectsInstaller.Install(connection, "hangfire", false);

            host.Run();
        }
    }
}