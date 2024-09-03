using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Delete;

namespace MyWallet.UnitTests.Features.Wallets.Delete;

public sealed class DeleteWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();

    private readonly DeleteWalletHandler sut;

    private static readonly DeleteWalletCommand Command = new(Constants.Wallet.Id.Value)
    {
        UserId = Constants.User.Id.Value
    };

    public DeleteWalletHandlerTests()
    {
        sut = new DeleteWalletHandler(walletRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDeleted()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await walletRepository
            .Received(1)
            .UpdateAsync(wallet, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnWalletNotFound()
    {
        // Arrange
        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(WalletErrors.NotFound);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.NotFound);
    }
}