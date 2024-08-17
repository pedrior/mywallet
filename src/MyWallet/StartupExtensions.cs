using Asp.Versioning;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;

namespace MyWallet;

public static class WebApplicationExtensions
{
    public static void UseEndpoints(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var versionedGroup = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);

        foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
        {
            endpoint.Build(versionedGroup);
        }
    }
    
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<MigrationWorker>()
            .PerformMigration();
    }
}