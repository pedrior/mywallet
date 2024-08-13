namespace MyWallet.Domain.Users;

public interface IPasswordHasher
{
    string Hash(Password password);
    
    bool Verify(Password password, string passwordHash);
}