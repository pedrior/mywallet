using System.Data;
using MyWallet.Domain.Transactions;

namespace MyWallet.Shared.Persistence.Types.Transactions;

public sealed class TransactionIdHandler : SqlMapper.TypeHandler<TransactionId>
{
    public override void SetValue(IDbDataParameter parameter, TransactionId? value) => 
        parameter.Value = value?.ToString();

    public override TransactionId? Parse(object? value)
    {
        return value is null
            ? null
            : new TransactionId(Ulid.Parse(value.ToString()));
    }
}