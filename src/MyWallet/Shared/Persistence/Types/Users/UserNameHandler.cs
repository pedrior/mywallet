using System.Data;
using MyWallet.Domain.Users;

namespace MyWallet.Shared.Persistence.Types.Users;

public sealed class UserNameHandler : SqlMapper.TypeHandler<UserName>
{
    
    public override void SetValue(IDbDataParameter parameter, UserName? value) =>
        parameter.Value = value?.ToString();

    public override UserName? Parse(object? value)
    {
        return value is null
            ? null
            : UserName.Create(value.ToString()!).Value;
    }
}