using System;
using DataContract.V1.Response;
using Domain.Model;
using CreditTransactionType = Domain.Model.CreditTransactionType;

namespace WalletServiceApi.Utilities
{
    internal class TransactionTypeConverter : ITransactionTypeConverter
    {
        public CreditTransactionType Convert(DataContract.V1.Request.CreditTransactionType creditTransactionType)
        {
            switch (creditTransactionType)
            {
                case DataContract.V1.Request.CreditTransactionType.Deposit:
                    return CreditTransactionType.Deposit;
                case DataContract.V1.Request.CreditTransactionType.Stake:
                    return CreditTransactionType.Stake;
                case DataContract.V1.Request.CreditTransactionType.Win:
                    return CreditTransactionType.Win;
                default:
                    throw new ArgumentOutOfRangeException(nameof(creditTransactionType), creditTransactionType, null);
            }
        }

        public CreditTransactionResult Convert(TransactionState transactionState)
        {
            switch (transactionState)
            {
                case TransactionState.Accepted:
                    return CreditTransactionResult.Accepted;
                case TransactionState.Rejected:
                    return CreditTransactionResult.Rejected;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transactionState), transactionState, null);
            }
        }
    }
}
