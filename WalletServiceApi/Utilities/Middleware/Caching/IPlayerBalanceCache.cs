using System;
using CSharpFunctionalExtensions;

namespace WalletServiceApi.Utilities.Middleware.Caching
{
    public interface IPlayerBalanceCache
    {
        void InsertPlayerBalance(Guid playerIdentifier, decimal balance);

        Maybe<decimal> TryGetCached(Guid playerIdentifier);

        void InvalidatePlayerBalance(Guid playerIdentifier);
    }
}
