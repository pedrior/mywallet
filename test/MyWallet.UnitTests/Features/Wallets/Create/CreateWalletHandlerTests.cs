using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Create;

namespace MyWallet.UnitTests.Features.Wallets.Create;

public sealed class CreateWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();

    private readonly CreateWalletHandler sut;

    private static readonly CreateWalletCommand Command = new()
    {
        Name = Constants.Wallet.Name.Value,
        Color = Constants.Wallet.Color.Value,
        Currency = Constants.Wallet.Currency.Name,
        UserId = Ulid.NewUlid()
    };

    public CreateWalletHandlerTests()
    {
        sut = new CreateWalletHandler(walletRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldCreateWalletAndAddToRepository()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        await walletRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Wallet>(
                    w => w.Name.Value == Command.Name
                         && w.Color.Value == Command.Color),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnWalletId()
    {
        // Arrange
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<WalletId>();
    }
}