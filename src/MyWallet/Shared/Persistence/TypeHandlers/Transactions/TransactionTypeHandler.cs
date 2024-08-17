using System.Data;
using MyWallet.Domain.Transactions;

namespace MyWallet.Shared.Persistence.TypeHandlers.Transactions;

public sealed class TransactionTypeHandler : SqlMapper.TypeHandler<TransactionType>
{
    public override void SetValue(IDbDataParameter parameter, TransactionType? value) => 
        parameter.Value = value?.Name;

    public override TransactionType? Parse(object? value)
    {
        return value is null
            ? null
            : TransactionType.FromName(value.ToString());
    }
}