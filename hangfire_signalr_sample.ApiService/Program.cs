using Hangfire;
using hangfire_signalr_sample.ApiService;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();
        builder.Services.AddControllers();
        builder.Services.AddSignalR();
        builder.Services.AddOpenApi();

        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });

        var connectioNString = builder.Configuration.GetConnectionString("database");
        if (!string.IsNullOrWhiteSpace(connectioNString))
        {
            builder.Services.AddHangfire(configuration => configuration
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                            .UseSimpleAssemblyNameTypeSerializer()
                            .UseRecommendedSerializerSettings()
                            .UseSqlServerStorage(connectioNString, new Hangfire.SqlServer.SqlServerStorageOptions()
                            {
                                SqlClientFactory = SqlClientFactory.Instance,
                                TryAutoDetectSchemaDependentOptions = false,
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.Zero,
                                UseRecommendedIsolationLevel = true,
                                DisableGlobalLocks = true, // Migration to Schema 7 is required
                                PrepareSchemaIfNecessary = true,
                                SchemaName = "hangfire"
                            }));
        }

        var app = builder.Build();
        app.UseRouting();
        app.UseStaticFiles();

        if (!string.IsNullOrWhiteSpace(connectioNString))
        {
            app.UseHangfireDashboard();
        }

        app.UseResponseCompression();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        app.MapDefaultEndpoints();

#pragma warning disable ASP0014 // Suggest using top level route registrations
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            if (!string.IsNullOrWhiteSpace(connectioNString))
            {
                endpoints.MapHangfireDashboard();
            }

            endpoints.MapHub<JobHub>("/jobs");
            endpoints.MapOpenApi();
        });
#pragma warning restore ASP0014 // Suggest using top level route registrations

        app.Run();
    }
}