using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Edit;
using WalletErrors = MyWallet.Features.Wallets.Shared.WalletErrors;

namespace MyWallet.UnitTests.Features.Wallets.Edit;

public sealed class EditWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();

    private readonly EditWalletHandler sut;

    private static readonly EditWalletCommand Command = new()
    {
        WalletId = Constants.Wallet.Id.Value,
        Name = Constants.Wallet.Name2.Value,
        Color = Constants.Wallet.Color2.Value,
        Currency = Constants.Wallet.Currency2.Name
    };

    public EditWalletHandlerTests()
    {
        sut = new EditWalletHandler(walletRepository);
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
    public async Task Handle_WhenEditFails_ShouldReturnError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        wallet.Archive(); // This will make the Edit fail

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenEditFails_ShouldNotUpdateWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        wallet.Archive(); // This will make the Edit fail

        A.CallTo(() => walletRepository.GetAsync(Constants.Wallet.Id, A<CancellationToken>._))
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => walletRepository.UpdateAsync(wallet, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}