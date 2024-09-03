using MyWallet.Domain.Transactions;
using MyWallet.Features.Transactions.Delete;
using TransactionErrors = MyWallet.Features.Transactions.Shared.TransactionErrors;

namespace MyWallet.UnitTests.Features.Transactions.Delete;

public sealed class DeleteTransactionHandlerTests
{
    private readonly ITransactionRepository transactionRepository = Substitute.For<ITransactionRepository>();

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
        transactionRepository.ExistsAsync(
                Arg.Is<TransactionId>(id => id.Value == Command.TransactionId),
                Arg.Any<CancellationToken>())
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
        transactionRepository.ExistsAsync(
                Arg.Is<TransactionId>(id => id.Value == Command.TransactionId),
                Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await transactionRepository
            .Received(1)
            .DeleteAsync(
                Arg.Is<TransactionId>(id => id.Value == Command.TransactionId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTransactionDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        transactionRepository.ExistsAsync(
                Arg.Is<TransactionId>(id => id.Value == Command.TransactionId),
                Arg.Any<CancellationToken>())
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
        transactionRepository.ExistsAsync(
                Arg.Is<TransactionId>(id => id.Value == Command.TransactionId),
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await sut.Handle(Command, CancellationToken.None);

        // Assert
        await transactionRepository
            .DidNotReceive()
            .DeleteAsync(
                Arg.Is<TransactionId>(id => id.Value == Command.TransactionId),
                Arg.Any<CancellationToken>());
    }
}