namespace MyWallet.Shared.Domain;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    
    DateTimeOffset? UpdatedAt { get; }
}