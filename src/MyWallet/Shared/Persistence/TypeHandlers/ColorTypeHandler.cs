using System.Data;
using MyWallet.Domain;

namespace MyWallet.Shared.Persistence.TypeHandlers;

public sealed class ColorTypeHandler : SqlMapper.TypeHandler<Color>
{
    public override void SetValue(IDbDataParameter parameter, Color? value) =>
        parameter.Value = value?.ToString();

    public override Color? Parse(object? value)
    {
        return value is null
            ? null
            : Color.Create(value.ToString()!).Value;
    }
}