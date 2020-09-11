using System;
using CSharpFunctionalExtensions;

namespace Domain.Model.ValueObjects
{
    public class Transaction : ValueObject<Transaction>
    {
        public Money Money { get; }
        public CreditTransactionType CreditTransactionType { get; }

        public Transaction(Money money, CreditTransactionType creditTransactionType)
        {
            Money = money;
            CreditTransactionType = creditTransactionType;
        }

        public Money GetBalanceAfterPerformed(Money initialAmount)
        {
            decimal addend = Money.Amount;
            int multiplier;

            switch (CreditTransactionType)
            {
                case CreditTransactionType.Deposit:
                case CreditTransactionType.Win:
                    multiplier = 1;
                    break;
                case CreditTransactionType.Stake:
                    multiplier = -1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("BalanceMovementType");
            }

            return Money.Create(addend * multiplier + initialAmount.Amount).Value;
        }

        protected override bool EqualsCore(Transaction other)
        {
            return Money.Equals(other.Money)
                   && CreditTransactionType == other.CreditTransactionType;
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                return 13 * Money.GetHashCode() * CreditTransactionType.GetHashCode();
            }
        }
    }
}
