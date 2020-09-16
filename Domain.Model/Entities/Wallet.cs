using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Model.Exceptions;
using Domain.Model.ValueObjects;

namespace Domain.Model.Entities
{
    public class Wallet : Entity
    {
        private readonly IList<ExecutedTransaction> _executedTransactions;

        public Wallet(Guid identifier) : base(identifier)
        {
            _executedTransactions = new List<ExecutedTransaction>();
        }

        public Money GetBalance()
        {
            return _executedTransactions.Aggregate(Money.Zero, (balance, movement) => movement.GetBalanceAfterPerformed(balance));
        }

        public bool IsMovementPossible(Money money, CreditTransactionType creditTransactionType)
        {
            Money currentBalance = GetBalance();

            Money balanceAfterMovement = new Transaction(money, creditTransactionType).GetBalanceAfterPerformed(currentBalance);

            return balanceAfterMovement.Amount >= 0;
        }

        public void MakeMovement(Money money, CreditTransactionType creditTransactionType)
        {
            if (!IsMovementPossible(money, creditTransactionType))
            {
                throw new NegativeBalanceReachedException("Balance would be negative after this transaction.");
            }

            _executedTransactions.Add(new ExecutedTransaction(money, creditTransactionType));
        }
    }
}
