using System.Data;

namespace MyWallet.Shared.Persistence.Types;

public sealed class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value) =>
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    
    public override DateOnly Parse(object? value)
    {
        return value is null
            ? DateOnly.MinValue
            : DateOnly.FromDateTime((DateTime)value);
    }
}