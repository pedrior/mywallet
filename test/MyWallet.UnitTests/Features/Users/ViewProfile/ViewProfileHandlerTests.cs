using MyWallet.Features.Users.ViewProfile;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Users.ViewProfile;

[TestSubject(typeof(ViewProfileHandler))]
public sealed class ViewProfileHandlerTests
{
    private readonly IDbContext dbContext = Substitute.For<IDbContext>();

    private readonly ViewProfileHandler sut;

    private static readonly ViewProfileQuery Query = new()
    {
        UserId = Ulid.NewUlid()
    };

    public ViewProfileHandlerTests()
    {
        sut = new ViewProfileHandler(dbContext);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnUserProfileResponseForCurrentUser()
    {
        // Arrange
        var userProfileResponse = new ViewProfileResponse
        {
            Name = "John Doe",
            Email = "john@doe.com",
            CreatedAt = DateTimeOffset.UtcNow.AddMonths(-2),
            UpdatedAt = null
        };

        dbContext.QuerySingleOrDefaultAsync<ViewProfileResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(userProfileResponse);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(userProfileResponse);
    }
}