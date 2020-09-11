using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace WalletServiceApi.Utilities.Middleware.Caching
{
    public class PlayerBalanceCache : IPlayerBalanceCache
    {
        private readonly Dictionary<Guid, decimal> _playerBalance = new Dictionary<Guid, decimal>();

        public void InsertPlayerBalance(Guid playerIdentifier, decimal balance)
        {
            _playerBalance[playerIdentifier] = balance;
        }

        public Maybe<decimal> TryGetCached(Guid playerIdentifier)
        {
            bool isCached = _playerBalance.TryGetValue(playerIdentifier, out decimal cachedValue);

            return isCached 
                ? Maybe<decimal>.From(cachedValue) 
                : Maybe<decimal>.None;
        }

        public void InvalidatePlayerBalance(Guid playerIdentifier)
        {
            _playerBalance.Remove(playerIdentifier);
        }
    }
}
