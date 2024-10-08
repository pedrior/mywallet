using System.Data;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.Types.Users;

public sealed class UserIdHandler : SqlMapper.TypeHandler<UserId>
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