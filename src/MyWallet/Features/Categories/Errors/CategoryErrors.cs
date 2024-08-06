namespace MyWallet.Features.Categories.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "category.not_found", description: "Category not found.");
}