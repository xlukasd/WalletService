using DataContract.V1.Response;
using Domain.Model;
using CreditTransactionType = Domain.Model.CreditTransactionType;

namespace WalletServiceApi.Utilities
{
    public interface ITransactionTypeConverter
    {
        CreditTransactionType Convert(DataContract.V1.Request.CreditTransactionType creditTransactionType);
        CreditTransactionResult Convert(TransactionState transactionState);
    }
}
