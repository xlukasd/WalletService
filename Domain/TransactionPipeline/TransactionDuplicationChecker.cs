using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Model;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain.TransactionPipeline
{
    internal class TransactionDuplicationChecker : ITransactionExecutor
    {
        private readonly IOneTransactionPerPlayerChecker _oneTransactionPerPlayerChecker;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Result<TransactionAttempt>>> _transactionTasksByIdentifier = new ConcurrentDictionary<Guid, TaskCompletionSource<Result<TransactionAttempt>>>();

        public TransactionDuplicationChecker(IOneTransactionPerPlayerChecker oneTransactionPerPlayerChecker)
        {
            _oneTransactionPerPlayerChecker = oneTransactionPerPlayerChecker;
        }

        public async Task<Result<TransactionAttempt>> ExecuteTransaction(Guid transactionIdentifier, Player player, Money money, CreditTransactionType creditTransactionType)
        {
            bool isFirstOccurenceOfTransaction = _transactionTasksByIdentifier.TryAdd(transactionIdentifier, new TaskCompletionSource<Result<TransactionAttempt>>());

            TaskCompletionSource<Result<TransactionAttempt>> transactionInProgress = _transactionTasksByIdentifier[transactionIdentifier];

            if (!isFirstOccurenceOfTransaction)
            {
                return await transactionInProgress.Task;
            }

            Result<TransactionAttempt> transaction = null;

            try
            {
                transaction = await _oneTransactionPerPlayerChecker.ExecuteTransaction(transactionIdentifier, player, money, creditTransactionType);
            }
            catch (Exception ex)
            {
                _transactionTasksByIdentifier.TryRemove(transactionIdentifier, out _);
                transaction = Result.Failure<TransactionAttempt>(ex.Message);
            }
            finally
            {
                transactionInProgress.SetResult(transaction);
            }

            return transaction;
        }
    }
}
