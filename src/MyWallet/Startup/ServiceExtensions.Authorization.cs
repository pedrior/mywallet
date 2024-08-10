using System.Reflection;
using MyWallet.Shared.Security;

namespace MyWallet.Startup;

public static partial class ServiceExtensions
{
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
}