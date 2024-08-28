using System.Reflection;
using System.Text;
using Asp.Versioning;
using Cysharp.Serialization.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Behaviors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Identity;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Persistence.Repositories;
using MyWallet.Shared.Persistence.Types;
using MyWallet.Shared.Persistence.Types.Categories;
using MyWallet.Shared.Persistence.Types.Transactions;
using MyWallet.Shared.Persistence.Types.Users;
using MyWallet.Shared.Persistence.Types.Wallets;
using MyWallet.Shared.Security;
using MyWallet.Shared.Security.Tokens;
using MyWallet.Shared.Serialization;
using MyWallet.Shared.Services;
using Serilog;

namespace MyWallet;

public static class ServiceExtensions
{
    private static readonly Assembly Assembly = typeof(ServiceExtensions).Assembly;

    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new UlidJsonConverter());
            options.SerializerOptions.Converters.Add(new TrimStringConverter());
        });

        services.AddAuthentication(configuration);

        services.AddAuthorization();

        services.AddSerilog(config => config
            .ReadFrom.Configuration(configuration));

        services.AddPersistence(configuration);

        services.AddEndpoints();

        services.AddValidatorsFromAssembly(Assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly);

            config.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            config.AddOpenRequestPreProcessor(typeof(CurrentUserBehavior<>));

            config.AddOpenBehavior(typeof(ExceptionBehavior<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .EnableApiVersionBinding();

        services.AddScoped<IUserContext, UserContext>();

        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddTransient<IEmailUniquenessChecker, EmailUniquenessChecker>();
        
        services.AddTransient<ITransactionService, TransactionService>();

        services.AddTransient<IDefaultCategoriesProvider, DefaultCategoriesProvider>();
    }

    private static void AddEndpoints(this IServiceCollection services)
    {
        var serviceDescriptors = GetAssemblyDefinedEndpoints
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);
    }

    private static IEnumerable<Type> GetAssemblyDefinedEndpoints =>
        Assembly.DefinedTypes.Where(type => type is { IsAbstract: false, IsInterface: false }
                                            && type.IsAssignableTo(typeof(IEndpoint)));
    
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
    
    private static void AddAuthorization(this IServiceCollection services)
    {
        PolicyServiceCollectionExtensions.AddAuthorization(services);

        RegisterGenericTypes(services, typeof(IAuthorizer<>), ServiceLifetime.Scoped);
        RegisterGenericTypes(services, typeof(IRequirementHandler<>), ServiceLifetime.Scoped);
    }

    private static void RegisterGenericTypes(this IServiceCollection services, Type genericType,
        ServiceLifetime lifetime)
    {
        var implementationTypes = GetTypesImplementingGenericType(Assembly, genericType);
        foreach (var implementationType in implementationTypes)
        {
            foreach (var interfaceType in implementationType.ImplementedInterfaces)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                }
            }
        }
    }

    private static List<TypeInfo> GetTypesImplementingGenericType(Assembly assembly, Type genericType)
    {
        if (!genericType.IsGenericType)
        {
            throw new ArgumentException("Must be a generic type", nameof(genericType));
        }

        return assembly.DefinedTypes
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType))
            .ToList();
    }
    
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
        SqlMapper.AddTypeHandler(new UlidHandler());
        SqlMapper.AddTypeHandler(new ColorHandler());
        SqlMapper.AddTypeHandler(new CurrencyHandler());
        SqlMapper.AddTypeHandler(new DateOnlyHandler());

        SqlMapper.AddTypeHandler(new UserIdHandler());
        SqlMapper.AddTypeHandler(new UserNameHandler());
        SqlMapper.AddTypeHandler(new EmailHandler());
        SqlMapper.AddTypeHandler(new PasswordHandler());

        SqlMapper.AddTypeHandler(new CategoryIdHandler());
        SqlMapper.AddTypeHandler(new CategoryTypeHandler());
        SqlMapper.AddTypeHandler(new CategoryNameHandler());
        
        SqlMapper.AddTypeHandler(new TransactionIdHandler());
        SqlMapper.AddTypeHandler(new TransactionNameHandler());
        SqlMapper.AddTypeHandler(new TransactionTypeHandler());

        SqlMapper.AddTypeHandler(new WalletIdHandler());
        SqlMapper.AddTypeHandler(new WalletNameHandler());
    }
}