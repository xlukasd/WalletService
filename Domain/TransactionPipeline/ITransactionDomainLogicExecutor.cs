using System;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain.TransactionPipeline
{
    internal interface ITransactionDomainLogicExecutor
    {
        Task<TransactionAttempt> ExecuteTransaction(Guid transactionIdentifier,
            Player player,
            Money money,
            CreditTransactionType creditTransactionType);
    }
}
