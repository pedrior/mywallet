using System.Data.Common;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using MyWallet.Shared.Persistence;

namespace MyWallet.IntegrationTests.Shared;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer npgsqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("tasker")
        .Build();

    private Respawner? respawner;
    private DbConnection? npgsqlConnection;

    public async Task InitializeAsync()
    {
        await npgsqlContainer.StartAsync();
        await InitializeRespawnerAsync();
    }

    public new Task DisposeAsync() => npgsqlContainer.DisposeAsync().AsTask();

    public HttpClient CreateClient(string? accessToken = null)
    {
        var client = base.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        return client;
    }

    public async Task ResetDatabaseAsync()
    {
        if (respawner is not null)
        {
            await respawner.ResetAsync(npgsqlConnection!);
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigurePersistenceServices);
    }

    private void ConfigurePersistenceServices(IServiceCollection services)
    {
        var connectionString = npgsqlContainer.GetConnectionString();

        services.RemoveAll<MigrationWorker>();
        services.RemoveAll<IDbContext>();

        services.AddTransient(_ => new MigrationWorker(connectionString));
        services.AddScoped<IDbContext>(_ => new DbContext(connectionString));
    }

    private async Task InitializeRespawnerAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<MigrationWorker>()
            .PerformMigration();

        npgsqlConnection = new NpgsqlConnection(npgsqlContainer.GetConnectionString());
        await npgsqlConnection.OpenAsync();

        respawner = await Respawner.CreateAsync(npgsqlConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }
}