using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Archive;
using NSubstitute.Extensions;

namespace MyWallet.UnitTests.Features.Wallets.Archive;

public sealed class ArchiveWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();

    private readonly ArchiveWalletHandler sut;

    private static readonly ArchiveWalletCommand Command = new()
    {
        WalletId = Constants.Wallet.Id.Value,
        UserId = Constants.User.Id.Value
    };

    public ArchiveWalletHandlerTests()
    {
        sut = new ArchiveWalletHandler(walletRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldArchiveWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        wallet.IsArchived.Should().BeTrue();
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
            .UpdateAsync(wallet,  Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnError()
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

    [Fact]
    public async Task Handle_WhenArchiveFails_ShouldReturnError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Simulate an error by archiving a wallet twice
        wallet.Archive();

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenArchiveFails_ShouldNotUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        walletRepository.GetAsync(Constants.Wallet.Id,  Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Simulate an error by archiving a wallet twice
        wallet.Archive();

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await walletRepository
            .DidNotReceive()
            .UpdateAsync(wallet,  Arg.Any<CancellationToken>());
    }
}