using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Persistence.Repositories;
using MyWallet.Shared.Persistence.TypeHandlers;
using MyWallet.Shared.Persistence.TypeHandlers.Categories;
using MyWallet.Shared.Persistence.TypeHandlers.Transactions;
using MyWallet.Shared.Persistence.TypeHandlers.Users;
using MyWallet.Shared.Persistence.TypeHandlers.Wallets;

namespace MyWallet.Startup;

public static partial class ServiceExtensions
{
    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDapperTypeHandling();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "'ConnectionStrings:DefaultConnection' not found in configuration.");
        }

        services.AddTransient(_ => new MigrationWorker(connectionString));

        services.AddScoped<IDbContext>(_ => new DbContext(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
    }

    private static void ConfigureDapperTypeHandling()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        SqlMapper.AddTypeHandler(new AmountHandler());
        SqlMapper.AddTypeHandler(new UlidTypeHandler());
        SqlMapper.AddTypeHandler(new ColorTypeHandler());
        SqlMapper.AddTypeHandler(new CurrencyHandler());

        SqlMapper.AddTypeHandler(new UserIdTypeHandler());
        SqlMapper.AddTypeHandler(new UserNameTypeHandler());
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new PasswordTypeHandler());

        SqlMapper.AddTypeHandler(new CategoryIdTypeHandler());
        SqlMapper.AddTypeHandler(new CategoryTypeTypeHandler());
        SqlMapper.AddTypeHandler(new CategoryNameTypeHandler());
        
        SqlMapper.AddTypeHandler(new TransactionIdHandler());
        SqlMapper.AddTypeHandler(new TransactionNameHandler());
        SqlMapper.AddTypeHandler(new TransactionTypeHandler());

        SqlMapper.AddTypeHandler(new WalletIdTypeHandler());
        SqlMapper.AddTypeHandler(new WalletNameTypeHandler());
    }
}