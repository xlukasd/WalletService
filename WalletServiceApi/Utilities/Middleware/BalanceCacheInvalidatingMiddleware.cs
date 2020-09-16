using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataContract.V1.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using WalletServiceApi.Utilities.Middleware.Caching;

namespace WalletServiceApi.Utilities.Middleware
{
    public class BalanceCacheInvalidatingMiddleware
    {
        private readonly IPlayerBalanceCache _playerBalanceCache;
        private readonly RequestDelegate _next;

        public BalanceCacheInvalidatingMiddleware(IPlayerBalanceCache playerBalanceCache, RequestDelegate next)
        {
            _playerBalanceCache = playerBalanceCache;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableRewind();

            Stream bodyStream = context.Request.Body;

            await InvalidateInCache(bodyStream);

            bodyStream.Position = 0;

            await _next(context);
        }

        private async Task InvalidateInCache(Stream bodyStream)
        {
            string bodyString;

            using (StreamReader reader = new StreamReader(bodyStream, Encoding.UTF8, true, 1024, true))
            {
                bodyString = await reader.ReadToEndAsync();
            }

            CreditTransactionDto creditTransactionDto = JsonConvert.DeserializeObject<CreditTransactionDto>(bodyString);

            _playerBalanceCache.InvalidatePlayerBalance(creditTransactionDto.PlayerIdentifier);
        }
    }
}
