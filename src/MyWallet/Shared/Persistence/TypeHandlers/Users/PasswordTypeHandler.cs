using System.Data;
using ValueObjects_Password = MyWallet.Domain.Users.Password;

namespace MyWallet.Shared.Persistence.TypeHandlers.Users;

public sealed class PasswordTypeHandler : SqlMapper.TypeHandler<ValueObjects_Password>
{
    public override void SetValue(IDbDataParameter parameter, ValueObjects_Password? value) =>
        parameter.Value = value?.ToString();

    public override ValueObjects_Password? Parse(object? value)
    {
        return value is null
            ? null
            : ValueObjects_Password.Create(value.ToString()!).Value;
    }
}