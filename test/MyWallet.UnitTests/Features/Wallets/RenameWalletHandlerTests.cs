using MyWallet.Domain.Wallets.Repository;
using MyWallet.Features.Wallets;

namespace MyWallet.UnitTests.Features.Wallets;

public sealed class RenameWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();

    private readonly RenameWalletHandler sut;

    private static readonly RenameWalletCommand Command = new()
    {
        WalletId = Constants.Wallet.Id.Value,
        Name = Constants.Wallet.Name2.Value
    };

    public RenameWalletHandlerTests()
    {
        sut = new RenameWalletHandler(walletRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert

        A.CallTo(() => walletRepository.UpdateAsync(wallet, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenRenameFails_ShouldReturnError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault(
            isArchived: true // This will make the rename fail
        );

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRenameFails_ShouldNotUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault(
            isArchived: true // This will make the rename fail
        );

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => walletRepository.UpdateAsync(wallet, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}