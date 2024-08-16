using Microsoft.IdentityModel.JsonWebTokens;
using MyWallet.Domain.Users;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security.Tokens;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users;

public sealed record LoginResponse
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}

public sealed record LoginCommand : ICommand<LoginResponse>
{
    public required string Email { get; init; }
    
    public required string Password { get; init; }
}

public sealed class LoginEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/login", LoginAsync);

    private static Task<IResult> LoginAsync(
        LoginCommand command,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(command, cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(c => c.Email)
            .MustSatisfyErrorValidation(Email.Validate);

        RuleFor(c => c.Password)
            .MustSatisfyErrorValidation(Password.Validate);
    }
}

public sealed class LoginHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ISecurityTokenProvider securityTokenProvider)
    : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<ErrorOr<LoginResponse>> Handle(LoginCommand command,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);
        var password = Password.Create(command.Password);

        var user = await userRepository.GetByEmailAsync(email.Value, cancellationToken);
        if (user is null || !user.VerifyPassword(password.Value, passwordHasher))
        {
            return Errors.UserErrors.InvalidCredentials;
        }

        var claims = CreateUserClaims(user);
        var securityToken = securityTokenProvider.GenerateToken(claims);

        return new LoginResponse
        {
            AccessToken = securityToken.AccessToken,
            ExpiresAt = securityToken.ExpiresAt
        };
    }

    private static Dictionary<string, object?> CreateUserClaims(User user) => new()
    {
        [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
        [JwtRegisteredClaimNames.Name] = user.Name.ToString(),
        [JwtRegisteredClaimNames.Email] = user.Email.ToString()
    };
}