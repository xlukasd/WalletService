using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            UseBalanceCacheInvalidatingMiddleware(app);
            UseBalanceCachingMiddleware(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void UseBalanceCachingMiddleware(IApplicationBuilder app)
        {
            Func<HttpContext, bool> isGetBalanceRequestFunc = context =>
                context.Request.Method == WebRequestMethods.Http.Get
                && context.Request.RouteValues.TryGetValue("action", out object action)
                && (action as string) == nameof(WalletController.GetBalance);

            app.UseWhen(isGetBalanceRequestFunc,
                appBuilder =>
                {
                    appBuilder.UseMiddleware<BalanceCachingMiddleware>();
                });
        }

        private void UseBalanceCacheInvalidatingMiddleware(IApplicationBuilder app)
        {
            Func<HttpContext, bool> isNewTransactionRequestFunc = context =>
                context.Request.Method == WebRequestMethods.Http.Post
                && context.Request.RouteValues.TryGetValue("action", out object action)
                && (action as string) == nameof(WalletController.Transaction);

            app.UseWhen(isNewTransactionRequestFunc,
                appBuilder =>
                {
                    appBuilder.UseMiddleware<BalanceCacheInvalidatingMiddleware>();
                });
        }
    }
}
