using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace hangfire_signalr_sample.WorkerService
{
    public sealed class HangfireActivator : JobActivator
    {
        private readonly Func<IHost> _hostFactory;

        public HangfireActivator(Func<IHost> hostFactory)
        {
            _hostFactory = hostFactory;
        }

        public override object ActivateJob(Type jobType)
        {
            var host = _hostFactory();

            return host.Services.GetRequiredService(jobType);
        }

        public override JobActivatorScope BeginScope(PerformContext context)
        {
            var host = _hostFactory();

            return new JobScope(host.Services.CreateScope());
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            var host = _hostFactory();

            return new JobScope(host.Services.CreateScope());
        }

        public override JobActivatorScope BeginScope()
        {
            var host = _hostFactory();

            return new JobScope(host.Services.CreateScope());
        }
    }
}