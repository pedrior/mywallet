using System.Data;
using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.Shared.Persistence.TypeHandlers.Users;

public sealed class UserIdTypeHandler : SqlMapper.TypeHandler<UserId>
{
    public override void SetValue(IDbDataParameter parameter, UserId? value) =>
        parameter.Value = value?.ToString();

    public override UserId? Parse(object? value)
    {
        return value is null
            ? null
            : new UserId(Ulid.Parse(value.ToString()));
    }
}