using MyWallet.Domain.Transactions;
using MyWallet.Domain.Wallets;
using MyWallet.Domain.Wallets.Events;

namespace MyWallet.Features.Wallets.Shared.Events;

public sealed class WalletDeletedEventHandler(
    IWalletRepository walletRepository,
    ITransactionRepository transactionRepository,
    ILogger<WalletDeletedEvent> logger) : IEventHandler<WalletDeletedEvent>
{
    public async Task Handle(WalletDeletedEvent e, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling domain event {@Event}", e);
        
        await transactionRepository.DeleteAllByWallet(e.WalletId, cancellationToken);
        await walletRepository.DeleteAsync(e.WalletId, cancellationToken);
    }
}