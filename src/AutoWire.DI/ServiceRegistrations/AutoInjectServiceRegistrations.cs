using System.Reflection;
using AutoWire.DI.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.ServiceRegistrations;

/// <summary>
/// Facilitates the registration of services within an <see cref="IServiceCollection"/> based on the presence of the <see cref="AutoInjectAttribute"/>.
/// </summary>
/// <remarks>
/// This class scans the specified assembly for types that are annotated with the <see cref="AutoInjectAttribute"/>.
/// It registers these types as services with the dependency injection container, allowing for easy and automatic service registration.
/// The registration utilizes the specified lifetime and optional key defined in the attribute.
/// </remarks>
public class AutoInjectServiceRegistrations
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoInjectServiceRegistrations"/> class.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where services will be registered.</param>
    public AutoInjectServiceRegistrations(IServiceCollection services) => _services = services;

    /// <summary>
    /// Registers services from the specified assembly that are marked with the <see cref="AutoInjectAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly from which to scan for services to register.</param>
    public void RegisterFromAssembly(Assembly assembly)
    {
        var typesWithAttribute = assembly.GetTypes()
            .Where(t => t is
            {
                IsAbstract: false,
                IsClass: true
            } && (t.GetCustomAttribute<AutoInjectAttribute>() != null));

        foreach (var type in typesWithAttribute) RegisterService(type);
    }

    private void RegisterService(Type type)
    {
        var autoInjectAttribute = type.GetCustomAttribute<AutoInjectAttribute>()!;
        var serviceType = type.GetInterfaces().FirstOrDefault() ?? type;
        var serviceDescriptor = Generate(serviceType, type, autoInjectAttribute);
        _services.Add(serviceDescriptor);
    }

    private static ServiceDescriptor Generate(
        Type serviceType,
        Type implementationType,
        AutoInjectAttribute autoInjectAttribute)
        => new(serviceType, autoInjectAttribute.Key, implementationType, autoInjectAttribute.Lifetime);
}
