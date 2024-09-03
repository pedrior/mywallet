using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Get;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Wallets.Get;

public sealed class GetWalletHandlerTests
{
    private readonly IDbContext db = Substitute.For<IDbContext>();

    private readonly GetWalletHandler sut;

    private static readonly GetWalletQuery Query = new(
        WalletId: Constants.Wallet.Id.Value);

    public GetWalletHandlerTests()
    {
        sut = new GetWalletHandler(db);
    }

    [Fact]
    public async Task Handle_WhenWalletExists_ShouldReturnWalletResponse()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        db.QuerySingleOrDefaultAsync<WalletResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(new WalletResponse
            {
                Id = wallet.Id.Value,
                Name = wallet.Name.Value,
                Color = wallet.Color.Value,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            });

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new
        {
            Id = wallet.Id.Value,
            Name = wallet.Name.Value,
            Color = wallet.Color.Value,
            wallet.CreatedAt,
            wallet.UpdatedAt
        });
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        db.QuerySingleOrDefaultAsync<WalletResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.NotFound);
    }
}