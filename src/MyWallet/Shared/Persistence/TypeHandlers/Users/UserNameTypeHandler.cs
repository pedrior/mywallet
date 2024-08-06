using System.Data;
using UserName = MyWallet.Domain.Users.ValueObjects.UserName;
using ValueObjects_UserName = MyWallet.Domain.Users.ValueObjects.UserName;

namespace MyWallet.Shared.Persistence.TypeHandlers.Users;

public sealed class UserNameTypeHandler : SqlMapper.TypeHandler<ValueObjects_UserName>
{
    public override void SetValue(IDbDataParameter parameter, ValueObjects_UserName? value) =>
        parameter.Value = value?.ToString();

    public override ValueObjects_UserName? Parse(object? value)
    {
        return value is null
            ? null
            : ValueObjects_UserName.Create(value.ToString()!).Value;
    }
}