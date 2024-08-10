using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Shared.Features;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users;

public sealed record UpdateProfileCommand : ICommand<Success>, IHaveUser
{
    public required string Name { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class UpdateProfileEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder)
    {
        builder.MapPost("users/me/profile", UpdateProfileAsync)
            .RequireAuthorization();
    }

    private static Task<IResult> UpdateProfileAsync(
        UpdateProfileCommand command,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
}

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(UserName.Validate);
    }
}

public sealed class UpdateProfileHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateProfileCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var name = UserName.Create(command.Name);
        
        var user = await userRepository.GetAsync(new(command.UserId), cancellationToken);
        user!.UpdateProfile(name.Value);

        await userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success;
    }
}