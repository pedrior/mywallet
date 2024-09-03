using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;

namespace MyWallet.Features.Transactions.Create;

public sealed class CreateTransactionHandler(
    ITransactionService transactionService,
    ITransactionRepository transactionRepository,
    IWalletRepository walletRepository,
    ICategoryRepository categoryRepository) : ICommandHandler<CreateTransactionCommand, Ulid>
{
    public async Task<ErrorOr<Ulid>> Handle(CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var type = TransactionType.FromName(command.Type);
        var name = TransactionName.Create(command.Name).Value;
        var amount = Amount.Create(command.Amount).Value;
        var currency = Currency.FromName(command.Currency);

        var wallet = await walletRepository.GetAsync(
            new WalletId(command.WalletId),
            cancellationToken);

        var category = await categoryRepository.GetAsync(
            new CategoryId(command.CategoryId),
            cancellationToken);

        var result = ErrorCombiner.Combine(wallet, category);
        if (result.IsError)
        {
            return result.Errors;
        }

        var transaction = transactionService.CreateTransaction(
            wallet.Value,
            category.Value,
            type,
            name,
            amount,
            currency,
            command.Date);

        return await transaction
            .ThenDoAsync(t => transactionRepository.AddAsync(t, cancellationToken))
            .Then(t => t.Id.Value);
    }
}