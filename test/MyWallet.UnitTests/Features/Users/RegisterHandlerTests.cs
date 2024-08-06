using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Repository;
using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Features.Users;

namespace MyWallet.UnitTests.Features.Users;

[TestSubject(typeof(RegisterHandler))]
public sealed class RegisterHandlerTests
{
    private readonly IUserRepository userRepository = A.Fake<IUserRepository>();
    private readonly IEmailUniquenessChecker emailUniquenessChecker = A.Fake<IEmailUniquenessChecker>();
    private readonly IPasswordHasher passwordHasher = A.Fake<IPasswordHasher>();

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

        A.CallTo(() => emailUniquenessChecker.IsUniqueAsync(email, A<CancellationToken>._))
            .Returns(true);

        A.CallTo(() => passwordHasher.Hash(password))
            .Returns(hashedPassword);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        A.CallTo(() => userRepository.AddAsync(
                A<User>.That.Matches(u => u.Email == email
                                          && u.Name == name
                                          && u.PasswordHash == hashedPassword),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}