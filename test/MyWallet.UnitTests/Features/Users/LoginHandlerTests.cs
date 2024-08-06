using Microsoft.IdentityModel.JsonWebTokens;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Users;
using MyWallet.Features.Users.Shared;
using MyWallet.Shared.Security.Tokens;

namespace MyWallet.UnitTests.Features.Users;

[TestSubject(typeof(LoginHandler))]
public sealed class LoginHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();
    private readonly IPasswordHasher passwordHasher = A.Fake<IPasswordHasher>();
    private readonly ISecurityTokenProvider securityTokenProvider = A.Fake<ISecurityTokenProvider>();

    private readonly LoginHandler sut;

    private static readonly LoginCommand Command = new()
    {
        Email = "john@doe.com",
        Password = "JohnDoe123"
    };

    public LoginHandlerTests()
    {
        sut = new LoginHandler(userRepository, passwordHasher, securityTokenProvider);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldReturnLoginResponse()
    {
        // Arrange
        var user = (await Factories.User.CreateDefault()).Value;

        var claims = new Dictionary<string, object?>
        {
            [JwtRegisteredClaimNames.Sub] = user.Id.ToString(),
            [JwtRegisteredClaimNames.Name] = user.Name.ToString(),
            [JwtRegisteredClaimNames.Email] = user.Email.ToString()
        };

        var securityToken = new SecurityToken(
            AccessToken: Guid.NewGuid().ToString(),
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(5));


        A.CallTo(() => userRepository.GetByEmailAsync(
                A<Email>.That.Matches(e => e.Value == Command.Email),
                A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => passwordHasher.Verify(
                A<Password>.That.Matches(p => p.Value == Command.Password),
                user.PasswordHash))
            .Returns(true);

        A.CallTo(() => securityTokenProvider.GenerateToken(claims))
            .Returns(securityToken);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new
        {
            securityToken.AccessToken,
            securityToken.ExpiresAt
        });
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        A.CallTo(() => userRepository.GetByEmailAsync(
                A<Email>.That.Matches(e => e.Value == Command.Email),
                A<CancellationToken>._))
            .Returns(null as User);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidCredentials);

        A.CallTo(securityTokenProvider)
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenPasswordVerificationFails_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        var user = (await Factories.User.CreateDefault()).Value;

        A.CallTo(() => userRepository.GetByEmailAsync(
                A<Email>.That.Matches(e => e.Value == Command.Email),
                A<CancellationToken>._))
            .Returns(user);

        A.CallTo(() => passwordHasher.Verify(
                A<Password>.That.Matches(p => p.Value == Command.Password),
                user.PasswordHash))
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.InvalidCredentials);

        A.CallTo(securityTokenProvider)
            .MustNotHaveHappened();
    }
}