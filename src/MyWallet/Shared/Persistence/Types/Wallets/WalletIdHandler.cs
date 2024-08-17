using System.Data;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Persistence.Types.Wallets;

public sealed class WalletIdHandler : SqlMapper.TypeHandler<WalletId>
{
    public override void SetValue(IDbDataParameter parameter, WalletId? value) =>
        parameter.Value = value?.ToString();

    public override WalletId? Parse(object? value)
    {
        return value is null
            ? null
            : new WalletId(Ulid.Parse(value.ToString()));
    }
}