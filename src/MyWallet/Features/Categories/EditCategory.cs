using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Errors;
using MyWallet.Features.Categories.Security;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Categories;

public sealed record EditCategoryRequest
{
    public required string Name { get; init; }

    public required string Color { get; init; }
}

public sealed record EditCategoryCommand : ICommand<Success>, IHaveUser
{
    public required Ulid CategoryId { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }
    
    public Ulid UserId { get; set; }
}

public sealed class EditCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPatch("categories/{id:length(26)}", EditCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> EditCategoryAsync(
        Ulid id,
        EditCategoryRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new EditCategoryCommand
        {
            CategoryId = id,
            Name = request.Name,
            Color = request.Color
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}

public sealed class EditCategoryValidator : AbstractValidator<EditCategoryCommand>
{
    public EditCategoryValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(CategoryName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);
    }
}

public sealed class EditCategoryAuthorizer : IAuthorizer<EditCategoryCommand>
{
    public IEnumerable<IRequirement> GetRequirements(EditCategoryCommand command)
    {
        yield return new CategoryOwnerRequirement(command.UserId, command.CategoryId);
    }
}

public sealed class EditCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<EditCategoryCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(EditCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(
            new CategoryId(command.CategoryId),
            cancellationToken);

        if (category is null)
        {
            return CategoryErrors.NotFound;
        }

        var name = CategoryName.Create(command.Name);
        var color = Color.Create(command.Color);

        category.Edit(
            name: name.Value,
            color: color.Value);

        await categoryRepository.UpdateAsync(category, cancellationToken);

        return Result.Success;
    }
}