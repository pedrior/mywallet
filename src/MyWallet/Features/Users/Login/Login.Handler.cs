using Microsoft.IdentityModel.JsonWebTokens;
using MyWallet.Domain.Users;
using MyWallet.Shared.Security.Tokens;
using UserErrors = MyWallet.Features.Users.Shared.UserErrors;

namespace MyWallet.Features.Users.Login;

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
        if (user.IsError || !user.Value.VerifyPassword(password.Value, passwordHasher))
        {
            return UserErrors.InvalidCredentials;
        }

        var claims = CreateUserClaims(user.Value);
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