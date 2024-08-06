using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MyWallet.Shared.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Ulid GetUserId(this ClaimsPrincipal principal)
    {
        if (!Ulid.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId))
        {
            throw new InvalidOperationException($"Claim '{JwtRegisteredClaimNames.Sub}' is missing or invalid.");
        }

        return userId;
    }
}