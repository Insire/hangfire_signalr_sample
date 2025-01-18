var builder = DistributedApplication.CreateBuilder(args);

// infrastructure

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

var bootstrapper = builder.AddProject<Projects.hangfire_signalr_sample_Bootstrapper>("bootstrapper")
    .WithReference(db)
    .WaitFor(db);

// backend

var apiService = builder.AddProject<Projects.hangfire_signalr_sample_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithReference(bootstrapper)
    .WaitFor(bootstrapper)
    .WithReference(db);

var workerService = builder.AddProject<Projects.hangfire_signalr_sample_WorkerService>("workerservice")
    .WithReference(db)
    .WithReference(apiService)
    .WaitFor(apiService);

// frontends

builder.AddProject<Projects.hangfire_signalr_sample_WpfClient>("wpfClient")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();