using MyWallet.Domain.Categories;

namespace MyWallet.UnitTests.Domain.Categories;

[TestSubject(typeof(DefaultCategoriesProvider))]
public sealed class DefaultCategoriesProviderTests
{
    [Fact]
    public async Task CreateDefaultCategoriesForUser_WhenCalled_ShouldReturnCategoriesForUser()
    {
        // Arrange
        var user = await Factories.User.CreateDefault();
        var provider = new DefaultCategoriesProvider();

        // Act
        var categories = provider.CreateDefaultCategoriesForUser(user.Value);

        // Assert
        categories.Should().AllSatisfy(category => category.UserId.Should().Be(user.Value.Id));
    }
}