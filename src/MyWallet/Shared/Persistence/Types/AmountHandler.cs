using System.Data;
using MyWallet.Domain;

namespace MyWallet.Shared.Persistence.Types;

public sealed class AmountHandler : SqlMapper.TypeHandler<Amount>
{
    public override void SetValue(IDbDataParameter parameter, Amount? value) =>
        parameter.Value = value?.Value;

    public override Amount? Parse(object? value)
    {
        return value is null
            ? null
            : Amount.Create((decimal)value).Value;
    }
}