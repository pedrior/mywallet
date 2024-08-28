using MyWallet.Domain.Transactions;
using MyWallet.Features.Transactions.Delete;
using TransactionErrors = MyWallet.Features.Transactions.Shared.TransactionErrors;

namespace MyWallet.UnitTests.Features.Transactions.Delete;

public sealed class DeleteTransactionHandlerTests
{
    private readonly ITransactionRepository transactionRepository = A.Fake<ITransactionRepository>();

    private readonly DeleteTransactionHandler sut;

    private static readonly DeleteTransactionCommand Command = new(Constants.Transaction.Id.Value);

    public DeleteTransactionHandlerTests()
    {
        sut = new DeleteTransactionHandler(transactionRepository);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnDeleted()
    {
        // Arrange
        A.CallTo(() => transactionRepository.ExistsAsync(
                A<TransactionId>.That.Matches(id => id.Value == Command.TransactionId),
                A<CancellationToken>.Ignored))
            .Returns(true);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldDeleteFromRepository()
    {
        // Arrange
        A.CallTo(() => transactionRepository.ExistsAsync(
                A<TransactionId>.That.Matches(id => id.Value == Command.TransactionId),
                A<CancellationToken>.Ignored))
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => transactionRepository.DeleteAsync(
                A<TransactionId>.That.Matches(id => id.Value == Command.TransactionId),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        A.CallTo(() => transactionRepository.ExistsAsync(
                A<TransactionId>.That.Matches(id => id.Value == Command.TransactionId),
                A<CancellationToken>.Ignored))
            .Returns(false);

        // Act
        var result = await sut.Handle(Command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TransactionErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldNotDeleteFromRepository()
    {
        // Arrange
        A.CallTo(() => transactionRepository.ExistsAsync(
                A<TransactionId>.That.Matches(id => id.Value == Command.TransactionId),
                A<CancellationToken>.Ignored))
            .Returns(false);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        A.CallTo(() => transactionRepository.DeleteAsync(
                A<TransactionId>.That.Matches(id => id.Value == Command.TransactionId),
                A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }
}