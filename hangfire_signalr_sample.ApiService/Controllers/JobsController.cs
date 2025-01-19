using Hangfire;
using hangfire_signalr_sample.ApiContracts;
using hangfire_signalr_sample.Jobs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hangfire_signalr_sample.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class JobsController : ControllerBase
    {
        private readonly IBackgroundJobClientV2 _backgroundJobClient;

        public JobsController(IBackgroundJobClientV2 backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        // POST api/<JobsController>
        [HttpPost]
        public void Post([FromBody] JobDto dto)
        {
            var value = dto.Value;
            var jobId = _backgroundJobClient.Enqueue<JobsService>((service) => service.Job(value));
            _backgroundJobClient.ContinueJobWith<JobsService>(jobId, (service) => service.PostJob(jobId));
        }
    }
}