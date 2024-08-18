using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Create;

public sealed class CreateTransactionHandler(
    ITransactionService transactionService,
    ITransactionRepository transactionRepository,
    IWalletRepository walletRepository,
    ICategoryRepository categoryRepository) : ICommandHandler<CreateTransactionCommand, Ulid>
{
    public async Task<ErrorOr<Ulid>> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(
            new WalletId(command.WalletId), cancellationToken);

        if (wallet is null)
        {
            return Shared.TransactionErrors.WalletNotFound;
        }
        
        var category = await categoryRepository.GetAsync(
            new CategoryId(command.CategoryId), cancellationToken);

        if (category is null)
        {
            return Shared.TransactionErrors.CategoryNotFound;
        }
        
        var type = TransactionType.FromName(command.Type);
        var name = TransactionName.Create(command.Name).Value;
        var amount = Amount.Create(command.Amount).Value;
        var currency = Currency.FromName(command.Currency);

        var result = transactionService.CreateTransaction(
            wallet,
            category,
            type,
            name,
            amount,
            currency,
            command.Date);

        return await result.ThenDoAsync(
                transaction => transactionRepository.AddAsync(transaction, cancellationToken))
            .Then(transaction => transaction.Id.Value);
    }
}