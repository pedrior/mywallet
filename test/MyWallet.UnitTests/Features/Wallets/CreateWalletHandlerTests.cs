using MyWallet.Domain.Wallets;
using MyWallet.Domain.Wallets.Repository;
using MyWallet.Domain.Wallets.ValueObjects;
using MyWallet.Features.Wallets;

namespace MyWallet.UnitTests.Features.Wallets;

public sealed class CreateWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();

    private readonly CreateWalletHandler sut;

    private static readonly CreateWalletCommand Command = new()
    {
        Name = Constants.Wallet.Name.Value,
        Color = Constants.Wallet.Color.Value,
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

        A.CallTo(() => walletRepository.AddAsync(
                A<Wallet>.That.Matches(
                    w => w.Name.Value == Command.Name
                         && w.Color.Value == Command.Color),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
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