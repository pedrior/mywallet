using Microsoft.IdentityModel.JsonWebTokens;
using MyWallet.Domain.Users;
using MyWallet.Features.Users.Login;
using MyWallet.Shared.Security.Tokens;

namespace MyWallet.UnitTests.Features.Users.Login;

[TestSubject(typeof(LoginHandler))]
public sealed class LoginHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ISecurityTokenProvider securityTokenProvider = Substitute.For<ISecurityTokenProvider>();

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

        userRepository.GetByEmailAsync(
                Arg.Is<Email>(e => e.Value == Command.Email),
                Arg.Any<CancellationToken>())
            .Returns(user);

        passwordHasher.Verify(
                Arg.Is<Password>(p => p.Value == Command.Password),
                user.PasswordHash)
            .Returns(true);

        securityTokenProvider.GenerateToken(Arg.Is<IDictionary<string, object?>>(
                // ReSharper disable UsageOfDefaultStructEquality
                d => d.SequenceEqual(claims)))
            // ReSharper restore UsageOfDefaultStructEquality
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
        userRepository.GetByEmailAsync(
                Arg.Is<Email>(e => e.Value == Command.Email),
                Arg.Any<CancellationToken>())
            .Returns(UserErrors.NotFound);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MyWallet.Features.Users.Shared.UserErrors.InvalidCredentials);

        securityTokenProvider
            .DidNotReceiveWithAnyArgs();
    }

    [Fact]
    public async Task Handle_WhenPasswordVerificationFails_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        var user = (await Factories.User.CreateDefault()).Value;

        userRepository.GetByEmailAsync(
                Arg.Is<Email>(e => e.Value == Command.Email),
                Arg.Any<CancellationToken>())
            .Returns(user);

        passwordHasher.Verify(
                Arg.Is<Password>(p => p.Value == Command.Password),
                user.PasswordHash)
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MyWallet.Features.Users.Shared.UserErrors.InvalidCredentials);

        securityTokenProvider
            .DidNotReceiveWithAnyArgs();
    }
}