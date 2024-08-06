namespace MyWallet.Domain.Users.Services;

public interface IPasswordHasher
{
    string Hash(ValueObjects.Password password);
    
    bool Verify(ValueObjects.Password password, string passwordHash);
}