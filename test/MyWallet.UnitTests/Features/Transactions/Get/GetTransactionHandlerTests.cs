using MyWallet.Domain.Transactions;
using MyWallet.Features.Transactions.Get;
using MyWallet.Shared.Persistence;

namespace MyWallet.UnitTests.Features.Transactions.Get;

[TestSubject(typeof(GetTransactionHandler))]
public sealed class GetTransactionHandlerTests
{
    private readonly IDbContext db = Substitute.For<IDbContext>();

    private readonly GetTransactionHandler sut;

    private static readonly GetTransactionQuery Query = new()
    {
        TransactionId = Constants.Transaction.Id.Value,
        UserId = Constants.User.Id.Value
    };

    public GetTransactionHandlerTests()
    {
        sut = new GetTransactionHandler(db);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnTransaction()
    {
        // Arrange
        var transaction = Factories.Transaction.CreateDefault().Value;
        var response = new GetTransactionResponse
        {
            Id = transaction.Id.Value,
            WalletId = transaction.WalletId.Value,
            CategoryId = transaction.CategoryId.Value,
            CategoryName = Constants.Category.Name.Value,
            Type = transaction.Type.Name,
            Name = transaction.Name.Value,
            Amount = transaction.Amount.Value,
            Currency = transaction.Currency.Name,
            Date = transaction.Date,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        db.QuerySingleOrDefaultAsync<GetTransactionResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        db.QuerySingleOrDefaultAsync<GetTransactionResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.NotFound);
    }
}