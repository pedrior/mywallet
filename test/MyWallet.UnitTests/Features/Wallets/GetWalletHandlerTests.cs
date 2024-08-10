using MyWallet.Features.Wallets;
using MyWallet.Features.Wallets.Shared.Errors;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Wallets;

public sealed class GetWalletHandlerTests
{
    private readonly IDbContext db = A.Fake<IDbContext>();

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

        A.CallTo(() => db.QuerySingleOrDefaultAsync<WalletResponse>(
                A<string>._,
                A<object>._,
                A<CancellationToken>._))
            .Returns(new WalletResponse
            {
                Id = wallet.Id.Value,
                Name = wallet.Name.Value,
                Color = wallet.Color.Value,
                IsDefault = wallet.IsDefault,
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
            wallet.IsDefault,
            wallet.CreatedAt,
            wallet.UpdatedAt
        });
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        A.CallTo(() => db.QuerySingleOrDefaultAsync<WalletResponse>(
                A<string>._,
                A<object>._,
                A<CancellationToken>._))
            .Returns(null as WalletResponse);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.NotFound);
    }
}