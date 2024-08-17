using System.Data;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Persistence.Types.Wallets;

public sealed class WalletNameHandler : SqlMapper.TypeHandler<WalletName>
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