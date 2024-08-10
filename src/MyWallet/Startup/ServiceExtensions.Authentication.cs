using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MyWallet.Shared.Security.Tokens;

namespace MyWallet.Startup;

public static partial class ServiceExtensions
{
    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddTransient<ISecurityTokenProvider, SecurityTokenProvider>();

        var securityTokenOptionsSection = configuration.GetRequiredSection(
            SecurityTokenOptions.SectionName);
        
        services.AddOptions<SecurityTokenOptions>()
            .Bind(securityTokenOptionsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var securityTokenOptions = securityTokenOptionsSection.Get<SecurityTokenOptions>()!;
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(securityTokenOptions.Key))
                };
            });
    }
}