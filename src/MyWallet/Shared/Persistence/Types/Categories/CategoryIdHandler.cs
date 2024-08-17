using System.Data;
using MyWallet.Domain.Categories;

namespace MyWallet.Shared.Persistence.Types.Categories;

public sealed class CategoryIdHandler : SqlMapper.TypeHandler<CategoryId>
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