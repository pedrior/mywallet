using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Enums;
using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.Repository;
using MyWallet.Features.Categories.Validations;
using MyWallet.Shared.Features;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Categories;

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
        CreateCategoryCommand command,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
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

public sealed class CreateCategoryHandler(
    IUserRepository userRepository,
    ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand, CategoryId>
{
    public async Task<ErrorOr<CategoryId>> Handle(CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(new(command.UserId), cancellationToken);

        var type = CategoryType.FromName(command.Type);
        var name = CategoryName.Create(command.Name);
        var color = Color.Create(command.Color);

        var category = Category.Create(
            id: CategoryId.New(),
            userId: user!.Id,
            type: type,
            name: name.Value,
            color: color.Value);

        return await user
            .AddCategory(category.Id)
            .ThenDoAsync(_ => categoryRepository.AddAsync(category, cancellationToken))
            .ThenDoAsync(_ => userRepository.UpdateAsync(user, cancellationToken))
            .Then(_ => category.Id);
    }
}