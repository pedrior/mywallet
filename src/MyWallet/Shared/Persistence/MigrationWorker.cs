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
            .LogToConsole()
            .Build();

        if (upgradeEngine.IsUpgradeRequired())
        {
            upgradeEngine.PerformUpgrade();
        }
    }
}