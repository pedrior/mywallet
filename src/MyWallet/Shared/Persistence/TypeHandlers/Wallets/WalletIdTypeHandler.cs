using System.Data;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.Shared.Persistence.TypeHandlers.Wallets;

public sealed class WalletIdTypeHandler : SqlMapper.TypeHandler<WalletId>
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