using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Unarchive;
using WalletErrors = MyWallet.Features.Wallets.Shared.WalletErrors;

namespace MyWallet.UnitTests.Features.Wallets.Unarchive;

public sealed class UnarchiveWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();

    private readonly UnarchiveWalletHandler sut;

    private static readonly UnarchiveWalletCommand Command = new()
    {
        WalletId = Constants.Wallet.Id.Value,
        UserId = Constants.User.Id.Value
    };

    public UnarchiveWalletHandlerTests()
    {
        sut = new UnarchiveWalletHandler(walletRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUnarchiveWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        wallet.IsArchived.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await walletRepository
            .Received(1)
            .UpdateAsync(wallet, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnError()
    {
        // Arrange
        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(null as Wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUnarchiveFails_ShouldReturnError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Simulate an error by unarchiving a wallet that is not archived
        wallet.Unarchive();

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUnarchiveFails_ShouldNotUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Simulate an error by archiving a wallet twice
        wallet.Unarchive();

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await walletRepository
            .DidNotReceive()
            .UpdateAsync(wallet, Arg.Any<CancellationToken>());
    }
}