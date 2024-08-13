using MyWallet.Domain.Wallets;

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
    public void Archive_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        // Act
        var result = wallet.Archive();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }
    
    [Fact]
    public void Archive_WhenCalled_ShouldArchiveWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        // Act
        wallet.Archive();

        // Assert
        wallet.IsArchived.Should().BeTrue();
        wallet.ArchivedAt.Should().BeCloseTo(
            DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        
        wallet.UpdatedAt.Should().BeCloseTo(
            DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Archive_WhenWalletIsArchived_ShouldReturnAlreadyArchivedError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        // Act
        var result = wallet.Archive();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.AlreadyArchived);
    }

    [Fact]
    public void Rename_WhenWalletIsArchived_ShouldReturnWalletIsArchivedError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();
        
        var newName = Constants.Wallet.Name2;

        // Act
        var result = wallet.Rename(newName);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.WalletIsArchived);
    }
}