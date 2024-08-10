using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Features.Categories.Errors;
using MyWallet.Features.Categories.Security;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;
using MyWallet.Shared.Errors;

namespace MyWallet.Features.Categories;

public sealed record DeleteCategoryCommand : ICommand<Deleted>, IHaveUser
{
    public required Ulid CategoryId { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapDelete("categories/{id:length(26)}", DeleteCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> DeleteCategoryAsync(
        Ulid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new DeleteCategoryCommand { CategoryId = id }, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
}

public sealed class DeleteCategoryAuthorizer : IAuthorizer<DeleteCategoryCommand>
{
    public IEnumerable<IRequirement> GetRequirements(DeleteCategoryCommand command)
    {
        yield return new CategoryOwnerRequirement(command.UserId, command.CategoryId);
    }
}

public sealed class DeleteCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(command.CategoryId);
        var category = await categoryRepository.GetAsync(categoryId, cancellationToken);

        if (category is null)
        {
            return CategoryErrors.NotFound;
        }

        category.Delete();

        await categoryRepository.UpdateAsync(category, cancellationToken);

        return Result.Deleted;
    }
}