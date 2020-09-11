using System;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain.TransactionPipeline
{
    internal class TransactionDomainLogicExecutor : ITransactionDomainLogicExecutor
    {
        public Task<TransactionAttempt> ExecuteTransaction(Guid transactionIdentifier, Player player, Money money, CreditTransactionType creditTransactionType)
        {
            bool isMovementPossible = player.Wallet.IsMovementPossible(money, creditTransactionType);
            TransactionState transactionResult;

            if (isMovementPossible)
            {
                player.Wallet.MakeMovement(money, creditTransactionType);
                transactionResult = TransactionState.Accepted;
            }
            else
            {
                transactionResult = TransactionState.Rejected;
            }

            return Task.FromResult(new TransactionAttempt(transactionIdentifier, transactionResult));
        }
    }
}
