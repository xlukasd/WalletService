using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DataContract.V1.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WalletServiceApi.Utilities.Middleware.Caching;

namespace WalletServiceApi.Utilities.Middleware
{
    public class BalanceCachingMiddleware
    {
        private readonly IPlayerBalanceCache _playerBalanceCache;
        private readonly RequestDelegate _next;

        public BalanceCachingMiddleware(IPlayerBalanceCache playerBalanceCache, RequestDelegate next)
        {
            _playerBalanceCache = playerBalanceCache;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Stream originalBody = context.Response.Body;

            try
            {
                Maybe<PlayerBalanceDto> playerBalanceDtoFromCache = TryGetValueFromCache(context);

                if (playerBalanceDtoFromCache.HasValue)
                {
                    await WriteValueToResponse(context.Response, playerBalanceDtoFromCache.Value);

                    return;
                }

                await using MemoryStream memStream = new MemoryStream();

                context.Response.Body = memStream;

                await _next(context);

                if (context.Response.StatusCode != (int) HttpStatusCode.OK)
                {
                    return;
                }

                memStream.Position = 0;

                await CacheResponse(context.Response.Body);

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        private async Task CacheResponse(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);

            string responseBody = await streamReader.ReadToEndAsync();

            PlayerBalanceDto playerBalanceDto = JsonConvert.DeserializeObject<PlayerBalanceDto>(responseBody);

            _playerBalanceCache.InsertPlayerBalance(playerBalanceDto.PlayerIdentifier, playerBalanceDto.Balance);
        }

        private Maybe<PlayerBalanceDto> TryGetValueFromCache(HttpContext context)
        {
            Guid playerIdentifier = Guid.Parse((string)context.Request.RouteValues[Constants.PlayerIdentifierArgumentName]);

            Maybe<decimal> cachedBalance = _playerBalanceCache.TryGetCached(playerIdentifier);

            if (cachedBalance.HasNoValue)
            {
                return Maybe<PlayerBalanceDto>.None;
            }

            PlayerBalanceDto playerBalanceDto = new PlayerBalanceDto()
            {
                PlayerIdentifier = playerIdentifier,
                Balance = cachedBalance.Value
            };

            return Maybe<PlayerBalanceDto>.From(playerBalanceDto);
        }

        private async Task WriteValueToResponse(HttpResponse httpResponse, PlayerBalanceDto playerBalanceDto)
        {
            string result = JsonConvert.SerializeObject(playerBalanceDto);

            httpResponse.ContentType = MediaTypeNames.Application.Json;
            httpResponse.StatusCode = (int)HttpStatusCode.OK;
            await httpResponse.WriteAsync(result);
        } 
    }
}
