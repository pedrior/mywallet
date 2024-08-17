using System.Data;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.TypeHandlers.Users;

public sealed class PasswordTypeHandler : SqlMapper.TypeHandler<Password>
{
    public override void SetValue(IDbDataParameter parameter, Password? value) =>
        parameter.Value = value?.ToString();

    public override Password? Parse(object? value)
    {
        return value is null
            ? null
            : Password.Create(value.ToString()!).Value;
    }
}