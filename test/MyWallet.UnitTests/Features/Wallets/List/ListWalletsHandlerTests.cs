using MyWallet.Features.Wallets.List;
using MyWallet.Shared.Contracts;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Wallets.List;

public sealed class ListWalletsHandlerTests
{
    private readonly IDbContext db = Substitute.For<IDbContext>();

    private readonly ListWalletsHandler sut;

    private static readonly ListWalletsQuery Query = new()
    {
        Page = 1,
        Limit = 10,
        UserId = Constants.User.Id.Value
    };

    public ListWalletsHandlerTests()
    {
        sut = new ListWalletsHandler(db);
    }

    [Fact]
    public async Task Handle_WhenUserHasWallets_ShouldReturnPageResponseWithWallets()
    {
        // Arrange
        WalletSummaryResponse[] wallets =
        [
            new()
            {
                Id = Ulid.NewUlid(),
                Name = "Wallet 1",
                Color = "#FF0000",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = Ulid.NewUlid(),
                Name = "Wallet 2",
                Color = "#00FF00",
                CreatedAt = DateTimeOffset.UtcNow
            }
        ];

        db.ExecuteScalarAsync<int>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(wallets.Length);

        db.QueryAsync<WalletSummaryResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(wallets);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(
            new PageResponse<WalletSummaryResponse>(
                Items: wallets,
                Page: Query.Page,
                Limit: Query.Limit,
                Total: wallets.Length));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotHaveWallets_ShouldReturnEmptyPageResponse()
    {
        // Arrange
        db.ExecuteScalarAsync<int>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(0);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(PageResponse<WalletSummaryResponse>.Empty(Query.Page, Query.Limit));

        await db
            .DidNotReceive()
            .QueryAsync<WalletSummaryResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>());
    }
}