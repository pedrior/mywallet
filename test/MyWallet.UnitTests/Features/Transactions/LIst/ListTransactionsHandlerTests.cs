using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.List;
using MyWallet.Shared.Persistence;
using TransactionErrors = MyWallet.Features.Transactions.Shared.TransactionErrors;

namespace MyWallet.UnitTests.Features.Transactions.LIst;

[TestSubject(typeof(ListTransactionsHandler))]
public sealed class ListTransactionsHandlerTests
{
    private readonly IWalletRepository walletRepository = Substitute.For<IWalletRepository>();
    private readonly IDbContext db = Substitute.For<IDbContext>();

    private readonly ListTransactionsHandler sut;

    private static readonly ListTransactionsQuery Query = new()
    {
        WalletId = Constants.Wallet.Id.Value,
        From = DateOnly.FromDateTime(DateTime.Now),
        To = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
        Page = 1,
        Limit = 3,
        UserId = Constants.User.Id.Value
    };

    public ListTransactionsHandlerTests()
    {
        sut = new ListTransactionsHandler(walletRepository, db);

        walletRepository.ExistsAsync(
                Arg.Is<WalletId>(v => v.Value == Query.WalletId),
                Arg.Any<CancellationToken>())
            .Returns(true);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnPageWithTransactions()
    {
        // Arrange
        var transactions = new List<TransactionResponse>
        {
            ToTransactionResponse(Factories.Transaction.CreateDefault().Value),
            ToTransactionResponse(Factories.Transaction.CreateDefault().Value),
            ToTransactionResponse(Factories.Transaction.CreateDefault().Value),
            ToTransactionResponse(Factories.Transaction.CreateDefault().Value),
            ToTransactionResponse(Factories.Transaction.CreateDefault().Value),
            ToTransactionResponse(Factories.Transaction.CreateDefault().Value)
        };

        db.ExecuteScalarAsync<int>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(transactions.Count);

        db.QueryAsync<TransactionResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(transactions[..3]);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new
        {
            Items = transactions[..3],
            Query.Page,
            Query.Limit,
            Total = transactions.Count,
            Query.WalletId,
            Query.From,
            Query.To
        });
    }

    [Fact]
    public async Task Handle_WhenNoTransactions_ShouldReturnEmptyPage()
    {
        // Arrange
        db.ExecuteScalarAsync<int>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>())
            .Returns(0);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new
        {
            Items = Array.Empty<TransactionResponse>(),
            Query.Page,
            Query.Limit,
            Total = 0,
            Query.WalletId,
            Query.From,
            Query.To
        });

        await db
            .DidNotReceive()
            .QueryAsync<TransactionResponse>(
                Arg.Any<string>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnError()
    {
        // Arrange
        walletRepository.ExistsAsync(
                Arg.Is<WalletId>(v => v.Value == Query.WalletId),
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await sut.Handle(Query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.WalletNotFound);
    }

    private static TransactionResponse ToTransactionResponse(Transaction transaction) => new()
    {
        Id = Ulid.NewUlid(),
        Type = transaction.Type.Name,
        Name = transaction.Name.Value,
        Category = Constants.Category.Name.Value,
        Amount = transaction.Amount.Value,
        Currency = transaction.Currency.Name,
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(1, 5)))
    };
}