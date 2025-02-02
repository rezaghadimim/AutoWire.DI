using System.Reflection;
using AutoWire.DI.Attributes;
using AutoWire.DI.Exceptions;
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
            .Where(t => !t.IsAbstract && (t.GetCustomAttribute<AutoInjectAttribute>() != null));

        foreach (var type in typesWithAttribute) RegisterService(type);
    }

    private void RegisterService(Type type)
    {
        var autoInjectAttribute = type.GetCustomAttribute<AutoInjectAttribute>()!;
        var implementedInterfaces = type.GetInterfaces();

        // Determine the appropriate service type
        var serviceType = autoInjectAttribute.ServiceType ?? DetermineDirectServiceType(type, implementedInterfaces);

        // Check for duplicate service registration
        var conflictingType = GetConflictingService(serviceType, autoInjectAttribute.Key);
        if (conflictingType != null)
        {
            throw new DuplicateServiceRegistrationException(type, conflictingType, serviceType, autoInjectAttribute.Key);
        }

        var serviceDescriptor = Generate(serviceType, type, autoInjectAttribute);
        _services.Add(serviceDescriptor);
    }

    private static Type DetermineDirectServiceType(Type type,
        Type[] implementedInterfaces)
    {
        // Filter out interfaces that are implemented by parent classes
        var directInterfaces = implementedInterfaces
            .Where(i => type.GetInterfaces().All(parent => parent == i || !parent.GetInterfaces().Contains(i)))
            .ToArray();

        return directInterfaces.Length switch
        {
            0 => type,
            1 => directInterfaces[0],
            _ => throw new AmbiguousServiceTypeException(type.FullName!)
        };
    }

    private Type? GetConflictingService(Type serviceType,
        string? key)
    {
        var existingRegistration = _services.FirstOrDefault(descriptor =>
            descriptor.ServiceType == serviceType &&
            descriptor.ImplementationType != null && // Ensure it's a class type
            descriptor.ImplementationInstance is AutoInjectAttribute existingAttribute &&
            existingAttribute.Key == key);

        return existingRegistration?.ImplementationType;
    }

    private static ServiceDescriptor Generate(
        Type serviceType,
        Type implementationType,
        AutoInjectAttribute autoInjectAttribute)
        => new(serviceType, autoInjectAttribute.Key, implementationType, autoInjectAttribute.Lifetime);
}
