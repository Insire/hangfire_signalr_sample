using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace hangfire_signalr_sample.WorkerService
{
    public sealed class JobScope : JobActivatorScope
    {
        private readonly IServiceScope _scope;

        public JobScope(IServiceScope scope)
        {
            _scope = scope;
        }

        public override object Resolve(Type type)
        {
            return _scope.ServiceProvider.GetRequiredService(type);
        }
    }
}