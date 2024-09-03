using MyWallet.Domain.Users;
using MyWallet.Features.Users.Register;

namespace MyWallet.UnitTests.Features.Users.Register;

[TestSubject(typeof(RegisterHandler))]
public sealed class RegisterHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IEmailUniquenessChecker emailUniquenessChecker = Substitute.For<IEmailUniquenessChecker>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();

    private readonly RegisterHandler sut;

    private static readonly RegisterCommand Command = new()
    {
        Name = "John Doe",
        Email = "john@doe.com",
        Password = "JohnDoe123"
    };

    public RegisterHandlerTests()
    {
        sut = new RegisterHandler(userRepository, emailUniquenessChecker, passwordHasher);
    }

    [Fact]
    public async Task Handle_HandleWhenCommandIsValid_ShouldCreateUser()
    {
        // Arrange
        var name = UserName.Create(Command.Name).Value;
        var email = Email.Create(Command.Email).Value;
        var password = Password.Create(Command.Password).Value;
        const string hashedPassword = "hashed-password";

        emailUniquenessChecker.IsUniqueAsync(email, Arg.Any<CancellationToken>())
            .Returns(true);

        passwordHasher.Hash(password)
            .Returns(hashedPassword);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        await userRepository
            .Received(1)
            .AddAsync(
                Arg.Is<User>(u => u.Email == email
                                  && u.Name == name
                                  && u.PasswordHash == hashedPassword),
                Arg.Any<CancellationToken>());
    }
}