namespace MyWallet.Shared.Domain;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; set; }
    
    DateTimeOffset? UpdatedAt { get; set; }
}