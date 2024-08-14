using MyWallet.Domain.Wallets;

namespace MyWallet.UnitTests.Domain.Wallets;

[TestSubject(typeof(Wallet))]
public sealed class WalletTests
{
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
    public void Unarchive_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        // Act
        var result = wallet.Unarchive();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void Unarchive_WhenCalled_ShouldUnarchiveWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        // Act
        wallet.Unarchive();

        // Assert
        wallet.IsArchived.Should().BeFalse();
        wallet.ArchivedAt.Should().BeNull();

        wallet.UpdatedAt.Should().BeCloseTo(
            DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Unarchive_WhenWalletIsNotArchived_ShouldReturnNotArchivedError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();

        // Act
        var result = wallet.Unarchive();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.NotArchived);
    }

    [Fact]
    public void Edit_WhenCalled_ShouldReturnSuccess()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        var newName = Constants.Wallet.Name2;
        var newColor = Constants.Wallet.Color2;
        var newCurrency = Constants.Wallet.Currency2;

        // Act
        var result = wallet.Edit(newName, newColor, newCurrency);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
    }

    [Fact]
    public void Edit_WhenCalled_ShouldChangeEditWallet()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        var newName = Constants.Wallet.Name2;
        var newColor = Constants.Wallet.Color2;
        var newCurrency = Constants.Wallet.Currency2;

        // Act
        wallet.Edit(newName, newColor, newCurrency);

        // Assert
        wallet.Name.Should().Be(newName);
        wallet.Color.Should().Be(newColor);
        wallet.Currency.Should().Be(newCurrency);
        wallet.UpdatedAt.Should().BeCloseTo(
            DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Edit_WhenWalletIsArchived_ShouldReturnWalletIsArchivedError()
    {
        // Arrange
        var wallet = Factories.Wallet.CreateDefault();
        wallet.Archive();

        var newName = Constants.Wallet.Name2;
        var newColor = Constants.Wallet.Color2;
        var newCurrency = Constants.Wallet.Currency2;

        // Act
        var result = wallet.Edit(newName, newColor, newCurrency);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(WalletErrors.WalletIsArchived);
    }
}