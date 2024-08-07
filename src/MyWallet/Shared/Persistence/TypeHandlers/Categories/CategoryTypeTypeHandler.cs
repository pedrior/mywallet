using System.Data;
using MyWallet.Domain.Categories.Enums;

namespace MyWallet.Shared.Persistence.TypeHandlers.Categories;

public sealed class CategoryTypeTypeHandler : SqlMapper.TypeHandler<CategoryType>
{
    public override void SetValue(IDbDataParameter parameter, CategoryType? value)
    {
        parameter.Value = value?.ToString();
    }

    public override CategoryType? Parse(object? value)
    {
        return value is null
            ? null
            : CategoryType.FromName(value.ToString());
    }
}