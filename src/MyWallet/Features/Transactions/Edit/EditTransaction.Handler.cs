using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;

namespace MyWallet.Features.Transactions.Edit;

public sealed class EditTransactionHandler(
    ITransactionRepository transactionRepository,
    IWalletRepository walletRepository,
    ICategoryRepository categoryRepository) : ICommandHandler<EditTransactionCommand, Updated>
{
    public async Task<ErrorOr<Updated>> Handle(EditTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetAsync(
            new TransactionId(command.TransactionId),
            cancellationToken);

        return await transaction
            .ThenDoOrFail(t => command.Name is null ? t : ChangeName(t, command.Name))
            .ThenDoOrFail(t => command.Amount is null ? t : ChangeAmount(t, command.Amount.Value))
            .ThenDoOrFail(t => command.Currency is null ? t : ChangeCurrency(t, command.Currency))
            .ThenDoOrFail(t => command.Date is null ? t : ChangeDate(t, command.Date.Value))
            .ThenDoOrFailAsync(t => ChangeWalletAsync(t, command.WalletId, cancellationToken))
            .ThenDoOrFailAsync(t => ChangeCategoryAsync(t, command.CategoryId, cancellationToken))
            .ThenDoAsync(t => transactionRepository.UpdateAsync(t, cancellationToken))
            .Then(_ => Result.Updated);
    }

    private static ErrorOr<Transaction> ChangeName(Transaction transaction, string name)
    {
        return TransactionName.Create(name)
            .ThenDo(transaction.ChangeName)
            .Then(_ => transaction);
    }

    private static ErrorOr<Transaction> ChangeAmount(Transaction transaction, decimal amount)
    {
        return Amount.Create(amount)
            .ThenDo(transaction.ChangeAmount)
            .Then(_ => transaction);
    }

    private static ErrorOr<Transaction> ChangeCurrency(Transaction transaction, string currency)
    {
        transaction.ChangeCurrency(Currency.FromName(currency, ignoreCase: true));
        return transaction;
    }

    private static ErrorOr<Transaction> ChangeDate(Transaction transaction, DateOnly date)
    {
        transaction.ChangeDate(date);
        return transaction;
    }

    private async Task<IErrorOr> ChangeWalletAsync(
        Transaction transaction,
        Ulid? walletId,
        CancellationToken cancellationToken)
    {
        if (walletId is null)
        {
            return ErrorOrFactory.From(Unit.Value);
        }

        var wallet = await walletRepository.GetAsync(
            new WalletId(walletId.Value),
            cancellationToken);

        return wallet.ThenDoOrFail(w => transaction.ChangeWallet(w));
    }

    private async Task<IErrorOr> ChangeCategoryAsync(
        Transaction transaction,
        Ulid? categoryId,
        CancellationToken cancellationToken)
    {
        if (categoryId is null)
        {
            return ErrorOrFactory.From(Unit.Value);
        }

        var category = await categoryRepository.GetAsync(new CategoryId(categoryId.Value),
            cancellationToken);

        return category
            .ThenDo(c => transaction.ChangeCategory(c))
            .Then(_ => transaction);
    }
}