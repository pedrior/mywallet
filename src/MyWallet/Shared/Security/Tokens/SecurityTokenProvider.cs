using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace MyWallet.Shared.Security.Tokens;

public sealed class SecurityTokenProvider(
    TimeProvider timeProvider,
    IOptions<SecurityTokenOptions> options) : ISecurityTokenProvider
{
    private const string Algorithm = SecurityAlgorithms.HmacSha256;

    private readonly SecurityTokenOptions options = options.Value;

    public SecurityToken GenerateToken(IDictionary<string, object?> claims)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = now.AddMinutes(options.ExpiresInMinutes);
        
        claims.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.Key)), Algorithm);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            NotBefore = now,
            Expires = expiresAt,
            Claims = claims,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JsonWebTokenHandler
        {
            MapInboundClaims = false,
            SetDefaultTimesOnTokenCreation = false
        };

        return new SecurityToken(
            AccessToken: tokenHandler.CreateToken(tokenDescriptor),
            expiresAt);
    }
}