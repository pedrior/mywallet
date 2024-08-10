using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Enums;
using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Categories.Validations;
using MyWallet.Shared.Features;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Categories;

public sealed record CreateCategoryRequest
{
    public required string Type { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }
}

public sealed record CreateCategoryCommand : ICommand<CategoryId>, IHaveUser
{
    public required string Type { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class CreateCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("categories", CreateCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> CreateCategoryAsync(
        CreateCategoryRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand
        {
            Type = request.Type,
            Name = request.Name,
            Color = request.Color
        };

        return sender.Send(command, cancellationToken)
            .ToResponseAsync(id => Results.CreatedAtRoute("GetCategory", new { id }));
    }
}

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Type)
            .CategoryType();

        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(CategoryName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);
    }
}

public sealed class CreateCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand, CategoryId>
{
    public async Task<ErrorOr<CategoryId>> Handle(CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var type = CategoryType.FromName(command.Type);
        var name = CategoryName.Create(command.Name);
        var color = Color.Create(command.Color);

        var category = Category.Create(
            id: CategoryId.New(),
            userId: new UserId(command.UserId),
            type: type,
            name: name.Value,
            color: color.Value);

        await categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }
}