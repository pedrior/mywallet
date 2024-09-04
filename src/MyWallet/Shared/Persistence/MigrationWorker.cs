using DbUp;

namespace MyWallet.Shared.Persistence;

public sealed class MigrationWorker(string connectionString)
{
    public void PerformMigration()
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var scriptsPath = $"{AppDomain.CurrentDomain.BaseDirectory}Shared/Persistence/Scripts";
        var upgradeEngine = DeployChanges.To.PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem(scriptsPath)
            .WithTransaction()
            .LogToConsole()
            .Build();

        if (!upgradeEngine.IsUpgradeRequired())
        {
            return;
        }

        var result = upgradeEngine.PerformUpgrade();
        if (!result.Successful)
        {
            throw new MigrationException("Database migration failed", result.Error);
        }
    }
}