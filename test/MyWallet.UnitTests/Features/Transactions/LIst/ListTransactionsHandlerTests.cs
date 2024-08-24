using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Features.Transactions.List;
using MyWallet.Shared.Persistence;
using TransactionErrors = MyWallet.Features.Transactions.Shared.TransactionErrors;

namespace MyWallet.UnitTests.Features.Transactions.LIst;

[TestSubject(typeof(ListTransactionsHandler))]
public sealed class ListTransactionsHandlerTests
{
    private readonly IWalletRepository walletRepository = A.Fake<IWalletRepository>();
    private readonly IDbContext db = A.Fake<IDbContext>();

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

        A.CallTo(() => walletRepository.ExistsAsync(
                A<WalletId>.That.Matches(v => v.Value == Query.WalletId),
                A<CancellationToken>._))
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

        A.CallTo(() => db.ExecuteScalarAsync<int>(
                A<string>.Ignored,
                A<object>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(transactions.Count);

        A.CallTo(() => db.QueryAsync<TransactionResponse>(
                A<string>.Ignored,
                A<object>.Ignored,
                A<CancellationToken>.Ignored))
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
        A.CallTo(() => db.ExecuteScalarAsync<int>(
                A<string>.Ignored,
                A<object>.Ignored,
                A<CancellationToken>.Ignored))
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

        A.CallTo(() => db.QueryAsync<TransactionResponse>(
                A<string>.Ignored,
                A<object>.Ignored,
                A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenWalletDoesNotExist_ShouldReturnError()
    {
        // Arrange
        A.CallTo(() => walletRepository.ExistsAsync(
                A<WalletId>.That.Matches(v => v.Value == Query.WalletId),
                A<CancellationToken>._))
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