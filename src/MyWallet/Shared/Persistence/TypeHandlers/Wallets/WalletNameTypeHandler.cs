using System.Data;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.Shared.Persistence.TypeHandlers.Wallets;

public sealed class WalletNameTypeHandler : SqlMapper.TypeHandler<WalletName>
{
    public override void SetValue(IDbDataParameter parameter, WalletName? value)
    {
        parameter.Value = value?.ToString();
    }

    public override WalletName? Parse(object? value)
    {
        return value is null
            ? null
            : WalletName.Create(value.ToString() ?? string.Empty).Value;
    }
}