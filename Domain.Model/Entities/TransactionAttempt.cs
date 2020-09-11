using System;

namespace Domain.Model.Entities
{
    public class TransactionAttempt : Entity
    {
        public DateTime Executed { get; }
        public TransactionState TransactionState { get; }
        
        public TransactionAttempt(Guid identifier, TransactionState transactionState) : base(identifier)
        {
            Executed = DateTime.UtcNow;
            TransactionState = transactionState;
        }
    }
}
