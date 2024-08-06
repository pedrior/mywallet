using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Users.Repository;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Persistence.Repositories;
using MyWallet.Shared.Persistence.TypeHandlers;
using MyWallet.Shared.Persistence.TypeHandlers.Categories;
using MyWallet.Shared.Persistence.TypeHandlers.Users;

namespace MyWallet;

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
    }

    private static void ConfigureDapperTypeHandling()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        SqlMapper.AddTypeHandler(new UlidTypeHandler());
        SqlMapper.AddTypeHandler(new ColorTypeHandler());

        SqlMapper.AddTypeHandler(new UserIdTypeHandler());
        SqlMapper.AddTypeHandler(new UserNameTypeHandler());
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new PasswordTypeHandler());

        SqlMapper.AddTypeHandler(new CategoryIdTypeHandler());
        SqlMapper.AddTypeHandler(new CategoryNameTypeHandler());
    }
}