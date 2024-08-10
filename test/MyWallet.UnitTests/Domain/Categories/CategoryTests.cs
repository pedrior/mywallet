using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Events;

namespace MyWallet.UnitTests.Domain.Categories;

[TestSubject(typeof(Category))]
public sealed class CategoryTests
{
    [Fact]
    public void Delete_WhenCalled_ShouldRaiseCategoryDeletedEvent()
    {
        // Arrange
        var category = Factories.Category.CreateDefault();
        
        // Act
        category.Delete();
        
        // Assert
        category.Events.Should().Contain(p => p is CategoryDeletedEvent);
    }
}