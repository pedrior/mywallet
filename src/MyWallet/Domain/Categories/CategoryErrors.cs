namespace MyWallet.Domain.Categories;

public static class CategoryErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "category.not_found",
        description: "The Category does not exist.");
}