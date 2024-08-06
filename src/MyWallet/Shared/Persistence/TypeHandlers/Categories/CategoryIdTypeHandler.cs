using System.Data;
using MyWallet.Domain.Categories.ValueObjects;

namespace MyWallet.Shared.Persistence.TypeHandlers.Categories;

public sealed class CategoryIdTypeHandler : SqlMapper.TypeHandler<CategoryId>
{
    public override void SetValue(IDbDataParameter parameter, CategoryId? value)
    {
        parameter.Value = value?.ToString();
    }

    public override CategoryId? Parse(object? value)
    {
        return value is null
            ? null
            : new CategoryId(Ulid.Parse(value.ToString()));
    }
}