using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Unarchive;
using WalletErrors = MyWallet.Features.Wallets.Shared.WalletErrors;

namespace MyWallet.UnitTests.Features.Wallets.Unarchive;

public sealed class UnarchiveWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();

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

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
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

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
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

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => walletRepository.UpdateAsync(wallet, A<CancellationToken>._))
            .MustHaveHappened();
    }
    
    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnError()
    {
        // Arrange
        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
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

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
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

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Simulate an error by archiving a wallet twice
        wallet.Unarchive();

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => walletRepository.UpdateAsync(wallet, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}