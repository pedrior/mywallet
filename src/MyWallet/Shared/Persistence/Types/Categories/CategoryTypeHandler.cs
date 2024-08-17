using System.Data;
using MyWallet.Domain.Categories;

namespace MyWallet.Shared.Persistence.Types.Categories;

public sealed class CategoryTypeHandler : SqlMapper.TypeHandler<CategoryType>
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