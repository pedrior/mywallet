using System.Reflection;
using Asp.Versioning;
using Cysharp.Serialization.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyWallet.Domain.Users;
using MyWallet.Shared.Behaviors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Identity;
using MyWallet.Shared.Serialization;
using MyWallet.Shared.Services;
using Serilog;

namespace MyWallet.Startup;

public static partial class ServiceExtensions
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
}