using System.Data;

namespace MyWallet.Shared.Persistence.TypeHandlers;

public sealed class UlidTypeHandler : SqlMapper.TypeHandler<Ulid>
{
    public override void SetValue(IDbDataParameter parameter, Ulid value)
    {
        parameter.Value = value.ToString();
    }

    public override Ulid Parse(object? value)
    {
        return value is null
            ? Ulid.Empty
            : Ulid.Parse(value.ToString());
    }
}