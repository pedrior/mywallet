using System.Data;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.Types.Users;

public sealed class PasswordHandler : SqlMapper.TypeHandler<Password>
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