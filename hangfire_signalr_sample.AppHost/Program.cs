
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

var bootstrapper = builder.AddProject<Projects.hangfire_signalr_sample_Bootstrapper>("bootstrapper")
    .WithReference(db)
    .WaitFor(db);

var apiService = builder.AddProject<Projects.hangfire_signalr_sample_ApiService>("apiservice")
    .WithReference(bootstrapper)
    .WaitFor(bootstrapper)
    .WithReference(db);

var workerService = builder.AddProject<Projects.hangfire_signalr_sample_WorkerService>("workerservice")
    .WithReference(db)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.hangfire_signalr_sample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();