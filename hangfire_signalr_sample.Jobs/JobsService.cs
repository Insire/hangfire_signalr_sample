using Hangfire;
using Microsoft.Extensions.Logging;

namespace hangfire_signalr_sample.Jobs
{
    public sealed class JobsService
    {
        private readonly ILogger<JobsService> _logger;

        public JobsService(ILogger<JobsService> logger)
        {
            _logger = logger;
        }

        [Queue("default")]
        public void Job(string value)
        {
            _logger.LogInformation("Job: {0}", value);
        }

        [Queue("default")]
        public void PostJob(string jobId)
        {
            _logger.LogInformation("PostJob: {0}", jobId);
        }
    }
}