using System.Data;
using MyWallet.Domain.Categories.ValueObjects;

namespace MyWallet.Shared.Persistence.TypeHandlers.Categories;

public sealed class CategoryNameTypeHandler : SqlMapper.TypeHandler<CategoryName>
{
    public override void SetValue(IDbDataParameter parameter, CategoryName? value) =>
        parameter.Value = value?.ToString();

    public override CategoryName? Parse(object? value)
    {
        return value is null
            ? null
            : CategoryName.Create(value.ToString()!).Value;
    }
}