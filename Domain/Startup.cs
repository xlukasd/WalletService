using Domain.TransactionPipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            BindTransactionPipeline(services);

            services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));
        }

        private void BindTransactionPipeline(IServiceCollection services)
        {
            services.AddSingleton<ITransactionExecutor, TransactionDuplicationChecker>();
            services.AddSingleton<IOneTransactionPerPlayerChecker, OneTransactionPerPlayerChecker>();
            services.AddSingleton<ITransactionDomainLogicExecutor, TransactionDomainLogicExecutor>();
        }
    }
}
