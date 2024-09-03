using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Edit;
using WalletErrors = MyWallet.Features.Wallets.Shared.WalletErrors;

namespace MyWallet.UnitTests.Features.Wallets.Edit;

public sealed class EditWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();

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

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
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
    public async Task Handle_WhenEditFails_ShouldReturnError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        wallet.Archive(); // This will make the Edit fail

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
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

        walletRepository.GetAsync(Constants.Wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await walletRepository
            .DidNotReceive()
            .UpdateAsync(wallet, Arg.Any<CancellationToken>());
    }
}