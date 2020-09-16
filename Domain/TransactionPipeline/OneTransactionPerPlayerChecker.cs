using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain.TransactionPipeline
{
    internal class OneTransactionPerPlayerChecker : IOneTransactionPerPlayerChecker
    {
        private const int MaxNumberOfTransactionsPerPlayer = 1;

        private readonly ITransactionDomainLogicExecutor _transactionDomainLogicExecutor;

        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _transactionQueuePerPlayer = new ConcurrentDictionary<Guid, SemaphoreSlim>();

        public OneTransactionPerPlayerChecker(ITransactionDomainLogicExecutor transactionDomainLogicExecutor)
        {
            _transactionDomainLogicExecutor = transactionDomainLogicExecutor;
        }

        public async Task<TransactionAttempt> ExecuteTransaction(Guid transactionIdentifier, Player player, Money money, CreditTransactionType creditTransactionType)
        {
            _transactionQueuePerPlayer.TryAdd(player.Identifier, new SemaphoreSlim(MaxNumberOfTransactionsPerPlayer, MaxNumberOfTransactionsPerPlayer));

            SemaphoreSlim semaphoreForTransactionProcessingPerPlayer = _transactionQueuePerPlayer[player.Identifier];

            await semaphoreForTransactionProcessingPerPlayer.WaitAsync();

            try
            {
                return await _transactionDomainLogicExecutor.ExecuteTransaction(transactionIdentifier, player, money, creditTransactionType);
            }
            finally
            {
                semaphoreForTransactionProcessingPerPlayer.Release(MaxNumberOfTransactionsPerPlayer);
            }
        }
    }
}
