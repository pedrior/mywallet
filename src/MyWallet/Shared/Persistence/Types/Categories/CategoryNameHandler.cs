using System.Data;
using MyWallet.Domain.Categories;

namespace MyWallet.Shared.Persistence.Types.Categories;

public sealed class CategoryNameHandler : SqlMapper.TypeHandler<CategoryName>
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