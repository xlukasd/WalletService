using System;

namespace DataContract.V1.Request
{
    public class CreditTransactionDto
    {
        public Guid TransactionIdentifier { get; set; }
        public Guid PlayerIdentifier { get; set; }
        public decimal Amount { get; set; }
        public CreditTransactionType Type { get; set; }
    }
}
