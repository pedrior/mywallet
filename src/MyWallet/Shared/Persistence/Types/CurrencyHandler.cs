using System.Data;
using MyWallet.Domain;

namespace MyWallet.Shared.Persistence.Types;

public sealed class CurrencyHandler : SqlMapper.TypeHandler<Currency>
{
    public override void SetValue(IDbDataParameter parameter, Currency? value) =>
        parameter.Value = value?.ToString();

    public override Currency? Parse(object? value)
    {
        return Currency.TryFromName(value?.ToString(), ignoreCase: true, out var currency)
            ? currency
            : null;
    }
}