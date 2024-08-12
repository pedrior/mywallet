using MyWallet.Domain.Wallets;
using MyWallet.Domain.Wallets.Errors;

namespace MyWallet.UnitTests.Domain.Wallets;

[TestSubject(typeof(Wallet))]
public sealed class WalletTests
{
    [Fact]
    public void Rename_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        var newName = Constants.Wallet.Name2;

        // Act
        var result = wallet.Rename(newName);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void Rename_WhenCalled_ShouldChangeName()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        var newName = Constants.Wallet.Name2;

        // Act
        wallet.Rename(newName);

        // Assert
        wallet.Name.Should().Be(newName);
        wallet.UpdatedAt.Should().BeCloseTo(
            DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Rename_WhenWalletIsArchived_ShouldReturnWalletIsArchivedError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault(isArchived: true);
        var newName = Constants.Wallet.Name2;

        // Act
        var result = wallet.Rename(newName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.WalletIsArchived);
    }
}