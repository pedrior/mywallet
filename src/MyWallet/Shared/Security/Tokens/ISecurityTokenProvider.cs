namespace MyWallet.Shared.Security.Tokens;

public interface ISecurityTokenProvider
{
    SecurityToken GenerateToken(IDictionary<string, object?> claims);
}