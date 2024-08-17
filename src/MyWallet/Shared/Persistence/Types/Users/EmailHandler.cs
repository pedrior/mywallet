using System.Data;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.Types.Users;

public sealed class EmailHandler : SqlMapper.TypeHandler<Email>
{
    public override void SetValue(IDbDataParameter parameter, Email? value) =>
        parameter.Value = value?.ToString();

    public override Email? Parse(object? value)
    {
        return value is null
            ? null
            : Email.Create(value.ToString()!).Value;
    }
}