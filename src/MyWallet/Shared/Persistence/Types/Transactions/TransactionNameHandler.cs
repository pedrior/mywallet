using System.Data;
using MyWallet.Domain.Transactions;

namespace MyWallet.Shared.Persistence.Types.Transactions;

public sealed class TransactionNameHandler : SqlMapper.TypeHandler<TransactionName>
{
    public override void SetValue(IDbDataParameter parameter, TransactionName? value) => 
        parameter.Value = value?.Value;

    public override TransactionName? Parse(object? value)
    {
        return value is null
            ? null
            : TransactionName.Create(value.ToString()!).Value;
    }
}