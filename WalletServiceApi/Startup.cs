using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletServiceApi.Controllers;
using WalletServiceApi.Utilities;
using WalletServiceApi.Utilities.Middleware;
using WalletServiceApi.Utilities.Middleware.Caching;

namespace WalletServiceApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IPlayerBalanceCache, PlayerBalanceCache>();
            services.AddSingleton<ITransactionTypeConverter, TransactionTypeConverter>();

            new Domain.Startup().ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWhen(context => context.Request.Method == System.Net.WebRequestMethods.Http.Get
                                   && context.Request.RouteValues.TryGetValue("action", out object action)
                                   && (action as string) == nameof(WalletController.GetBalance),
                appBuilder =>
                {
                    appBuilder.UseMiddleware<BalanceCaching>();
                });

            app.UseWhen(context => context.Request.Method == System.Net.WebRequestMethods.Http.Post
                                   && context.Request.RouteValues.TryGetValue("action", out object action)
                                   && (action as string) == nameof(WalletController.Transaction),
                appBuilder =>
                {
                    appBuilder.UseMiddleware<BalanceInvalidating>();
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
