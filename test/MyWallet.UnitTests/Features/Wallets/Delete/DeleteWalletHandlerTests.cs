using MyWallet.Domain.Wallets;
using MyWallet.Features.Wallets.Delete;

namespace MyWallet.UnitTests.Features.Wallets.Delete;

public sealed class DeleteWalletHandlerTests
{
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();

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
        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldDeleteWallet()
    {
        // Arrange
        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => walletRepository.DeleteAsync(new WalletId(Command.WalletId), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}