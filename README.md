# hangfire_signalr_sample

This repository showcases how an asp.net core web api can delegate work to an hangfire job and wait for it to complete before completing the http request that triggered the job.

## dependencies

- MS Sql Server (hangfire storage)
- Docker (runs the database)
- Aspire (development orchestration)
- WPF (frontend for sending the http request)
- SignalR (back channel notification for job completion)

## todo

- strongly typed signalr client
- (otel) tracing