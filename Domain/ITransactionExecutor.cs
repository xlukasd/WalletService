using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Model;
using Domain.Model.Entities;
using Domain.Model.ValueObjects;

namespace Domain
{
    public interface ITransactionExecutor
    {
        Task<Result<TransactionAttempt>> ExecuteTransaction(Guid transactionIdentifier,
            Player player,
            Money money,
            CreditTransactionType creditTransactionType);
    }
}
