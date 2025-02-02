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
    /// <exception cref="DuplicateServiceRegistrationException">
    /// Thrown if a service with the same type and key is already registered in the container.
    /// This prevents duplicate service registrations with the same key or type.
    /// </exception>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown when the class implements multiple interfaces and no explicit service type is provided in the attribute.
    /// This avoids ambiguity in service registration by ensuring only one type is registered as a service.
    /// </exception>
    public void RegisterFromAssembly(Assembly assembly)
    {
        // Retrieves all types in the assembly that are not abstract and are marked with the AutoInjectAttribute
        var typesWithAttribute = assembly.GetTypes()
            .Where(t => !t.IsAbstract && (t.GetCustomAttribute<AutoInjectAttribute>() != null));

        // Registers each service type found with the AutoInject attribute
        foreach (var type in typesWithAttribute)
        {
            RegisterService(type);
        }
    }

    /// <summary>
    /// Registers a service type in the dependency injection container.
    /// It first checks if the provided type is decorated with the <see cref="AutoInjectAttribute"/>.
    /// If the service type is not provided via the attribute, it attempts to determine it based on the implemented interfaces.
    /// Additionally, it checks for any conflicts (duplicate registrations) and throws an exception if a conflict is found.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> of the service to be registered.
    /// This type must be decorated with the <see cref="AutoInjectAttribute"/> to be registered.
    /// </param>
    /// <exception cref="DuplicateServiceRegistrationException">
    /// Thrown if a service with the same type and key is already registered in the container.
    /// This prevents duplicate service registrations with the same key or type.
    /// </exception>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown when the class implements multiple interfaces and no explicit service type is provided in the attribute.
    /// This avoids ambiguity in service registration by ensuring only one type is registered as a service.
    /// </exception>
    protected internal void RegisterService(Type type)
    {
        // Retrieve the AutoInject attribute from the type
        var autoInjectAttribute = type.GetCustomAttribute<AutoInjectAttribute>()!;
        var implementedInterfaces = type.GetInterfaces();

        // Determine the appropriate service type
        var serviceType = autoInjectAttribute.ServiceType ?? DetermineDirectServiceType(type, implementedInterfaces);

        // Check for duplicate service registration
        var conflictingType = GetConflictingService(serviceType, autoInjectAttribute.Key);
        if (conflictingType != null)
        {
            // If a duplicate is found, throw an exception
            throw new DuplicateServiceRegistrationException(type, conflictingType, serviceType, autoInjectAttribute.Key);
        }

        // Generate the service descriptor and register the service
        var serviceDescriptor = Generate(serviceType, type, autoInjectAttribute);
        _services.Add(serviceDescriptor);
    }

    /// <summary>
    /// Determines the direct service type based on the implemented interfaces, excluding inherited ones.
    /// If no direct interface is found, it falls back to using the class itself.
    /// </summary>
    /// <param name="type">The type to examine for implemented interfaces.</param>
    /// <param name="implementedInterfaces">The interfaces implemented by the class.</param>
    /// <returns>The appropriate service type.</returns>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown when the class implements multiple interfaces, but no explicit service type is provided in the attribute.
    /// </exception>
    private static Type DetermineDirectServiceType(Type type, Type[] implementedInterfaces)
    {
        // Get the direct interfaces implemented by the class, excluding inherited ones
        var directInterfaces = implementedInterfaces
            .Where(i => type.GetInterfaces().Contains(i) && !type.BaseType?.GetInterfaces().Contains(i) == true)
            .ToArray();

        // If no direct interface is found, fall back to using the class itself
        return directInterfaces.Length switch
        {
            0 => type, // Fallback to the class type if no interfaces are found
            1 => directInterfaces[0], // Return the single direct interface
            _ => throw new AmbiguousServiceTypeException(type.FullName!) // Throw exception if multiple interfaces are found
        };
    }

    /// <summary>
    /// Checks for any conflicting service registration in the container.
    /// A conflict is found if the same service type and key are already registered.
    /// </summary>
    /// <param name="serviceType">The service type to check for conflicts.</param>
    /// <param name="key">The key associated with the service (if any).</param>
    /// <returns>The conflicting service type, or null if no conflict is found.</returns>
    private Type? GetConflictingService(Type serviceType, string? key)
    {
        var existingRegistration = _services.FirstOrDefault(descriptor =>
            descriptor.ServiceType == serviceType &&
            descriptor.ImplementationType != null && // Ensure it's a class type
            descriptor.ImplementationType.GetCustomAttribute<AutoInjectAttribute>()?.Key == key);

        return existingRegistration?.ImplementationType;
    }

    /// <summary>
    /// Generates a ServiceDescriptor for the service, including the service type, implementation type, key, and lifetime.
    /// </summary>
    /// <param name="serviceType">The service type to register.</param>
    /// <param name="implementationType">The implementation type to register.</param>
    /// <param name="autoInjectAttribute">The AutoInject attribute containing the key and lifetime.</param>
    /// <returns>A ServiceDescriptor for the service.</returns>
    private static ServiceDescriptor Generate(Type serviceType, Type implementationType, AutoInjectAttribute autoInjectAttribute)
        => new(serviceType, autoInjectAttribute.Key, implementationType, autoInjectAttribute.Lifetime);
}
