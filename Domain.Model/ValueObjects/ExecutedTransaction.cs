using System;

namespace Domain.Model.ValueObjects
{
    public class ExecutedTransaction : Transaction
    {
        public DateTime MovementDate { get; }

        public ExecutedTransaction(Money money, CreditTransactionType creditTransactionType) : base(money, creditTransactionType)
        {
            MovementDate = DateTime.UtcNow;
        }

        protected override bool EqualsCore(Transaction other)
        {
            ExecutedTransaction executedTransaction = other as ExecutedTransaction;

            return executedTransaction != null
                   && MovementDate == executedTransaction.MovementDate
                   && base.Equals(other);
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                return base.GetHashCodeCore() * MovementDate.GetHashCode();
            }
        }
    }
}
