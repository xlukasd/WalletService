using System;

namespace DataContract.V1.Response
{
    public class TransactionResultDto
    {
        public Guid Identifier { get; set; }
        public CreditTransactionResult Result { get; set; }
    }
}
