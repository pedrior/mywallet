using MyWallet.Domain.Users;
using MyWallet.Shared.Security.Tokens;

namespace MyWallet.IntegrationTests.Shared;

public abstract class IntegrationTest(TestApplicationFactory app)
    : IClassFixture<TestApplicationFactory>, IAsyncLifetime
{
    private readonly IServiceScope serviceScope = app.Services.CreateScope();
    
    protected IServiceProvider Services => serviceScope.ServiceProvider;

    protected HttpClient CreateClient(string? accessToken = null) => app.CreateClient(accessToken);
    
    protected T GetRequiredService<T>() where T : notnull => Services.GetRequiredService<T>();

    protected string CreateAccessToken(User user)
    {
        var claims = new Dictionary<string, object?>
        {
            [JwtRegisteredClaimNames.Sub] = user.Id.Value.ToString(),
            [JwtRegisteredClaimNames.Name] = user.Name.Value,
            [JwtRegisteredClaimNames.Email] = user.Email.Value
        };
        
        var securityTokenProvider = GetRequiredService<ISecurityTokenProvider>();
        var securityToken = securityTokenProvider.GenerateToken(claims);
        
        return securityToken.AccessToken;
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        serviceScope.Dispose();
        await app.ResetDatabaseAsync();
    }
}