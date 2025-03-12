using System.Reflection;
using AutoWire.DI.Attributes;
using AutoWire.DI.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.ServiceRegistrations;

/// <summary>
/// Handles the automatic registration of services decorated with <see cref="AutoInjectAttribute"/>.
/// </summary>
/// <remarks>
/// Scans the given assembly for classes marked with <see cref="AutoInjectAttribute"/> and registers them
/// in the provided <see cref="IServiceCollection"/>.
/// </remarks>
public class ServiceRegistrar
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRegistrar"/> class.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where services will be registered.</param>
    public ServiceRegistrar(IServiceCollection services) => _services = services;

    /// <summary>
    /// Registers all services in the specified assembly that are marked with <see cref="AutoInjectAttribute"/>.
    /// </summary>
    /// <param name="assembly">The assembly to scan for injectable services.</param>
    /// <exception cref="DuplicateServiceRegistrationException">
    /// Thrown if a service with the same type and key is already registered.
    /// </exception>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown when a class implements multiple interfaces and the service type is not explicitly provided.
    /// </exception>
    public void RegisterFromAssembly(Assembly assembly)
    {
        var typesWithAttribute = assembly.GetTypes()
            .Where(t => !t.IsAbstract &&
                        (t.GetCustomAttribute<AutoInjectAttribute>() != null || t.GetCustomAttribute<KeyedAutoInjectAttribute>() != null));

        foreach (var type in typesWithAttribute)
        {
            RegisterService(type);
        }
    }

    /// <summary>
    /// Registers a service type in the dependency injection container based on the provided class type.
    /// </summary>
    /// <param name="type">The class type to register.</param>
    /// <exception cref="DuplicateServiceRegistrationException">
    /// Thrown if a service with the same type and key already exists.
    /// </exception>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown when multiple interfaces are implemented but no explicit service type is specified.
    /// </exception>
    protected internal void RegisterService(Type type)
    {
        var key = GetKey(type);

        var autoInjectAttribute = type.GetCustomAttribute<AutoInjectAttribute>()!;
        var serviceType = DetermineServiceType(type, type.GetCustomAttribute<AutoInjectAttribute>()!);

        ValidateNoConflict(serviceType, key, type);

        _services.Add(Generate(serviceType, key, type, autoInjectAttribute.Lifetime));
    }

    private static string? GetKey(Type type)
    {
        if (type.GetCustomAttribute<KeyedAutoInjectAttribute>() != null)
        {
            return type.Name;
        }

        var keyedAutoInjectAttribute = type.GetCustomAttribute<KeyedAutoInjectAttribute>();
        return keyedAutoInjectAttribute?.Key;
    }

    /// <summary>
    /// Determines the appropriate service type based on implemented interfaces or the class itself.
    /// </summary>
    /// <param name="type">The class type to analyze.</param>
    /// <param name="autoInjectAttribute">The <see cref="AutoInjectAttribute"/> associated with the class.</param>
    /// <returns>The resolved service type.</returns>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown if multiple interfaces are implemented without explicitly specifying a service type.
    /// </exception>
    private static Type DetermineServiceType(Type type,
        AutoInjectAttribute autoInjectAttribute)
    {
        var implementedInterfaces = type.GetInterfaces();
        var serviceType = autoInjectAttribute.ServiceType ?? GetDirectServiceType(type, implementedInterfaces);
        return serviceType.IsGenericType && autoInjectAttribute.ServiceType is null
            ? serviceType.GetGenericTypeDefinition()
            : serviceType;
    }

    /// <summary>
    /// Retrieves the direct service type implemented by the class.
    /// </summary>
    /// <param name="type">The class type to analyze.</param>
    /// <param name="implementedInterfaces">The interfaces implemented by the class.</param>
    /// <returns>The direct service type.</returns>
    /// <exception cref="AmbiguousServiceTypeException">
    /// Thrown if no unique direct interface can be identified.
    /// </exception>
    private static Type GetDirectServiceType(Type type,
        Type[] implementedInterfaces)
    {
        var directInterfaces = implementedInterfaces
            .Where(i => implementedInterfaces.All(other => other == i || !i.IsAssignableFrom(other)))
            .ToArray();

        return directInterfaces.Length switch
        {
            0 => type,
            1 => directInterfaces[0],
            _ => throw new AmbiguousServiceTypeException(type.FullName!)
        };
    }

    /// <summary>
    /// Ensures no conflicting service registrations exist.
    /// </summary>
    /// <param name="serviceType">The service type to validate.</param>
    /// <param name="key">An optional key for keyed services.</param>
    /// <param name="type">The type being registered.</param>
    /// <exception cref="DuplicateServiceRegistrationException">
    /// Thrown when a conflicting service registration is found.
    /// </exception>
    private void ValidateNoConflict(Type serviceType,
        string? key,
        Type type)
    {
        var conflictingType = GetConflictingService(serviceType, key);
        if (conflictingType != null)
        {
            throw new DuplicateServiceRegistrationException(type, conflictingType, serviceType, key);
        }
    }

    /// <summary>
    /// Checks if a service with the given type and key is already registered.
    /// </summary>
    /// <param name="serviceType">The service type to check.</param>
    /// <param name="key">An optional key for keyed services.</param>
    /// <returns>The conflicting service type if found, otherwise null.</returns>
    private Type? GetConflictingService(Type serviceType,
        string? key) => key is null
        ? _services.FirstOrDefault(descriptor => descriptor.ServiceType == serviceType)?.ImplementationType
        : _services.FirstOrDefault(descriptor =>
                descriptor.ServiceType == serviceType &&
                descriptor.KeyedImplementationType != null &&
                descriptor.KeyedImplementationType.GetCustomAttribute<AutoInjectAttribute>()?.Key == key)
            ?.KeyedImplementationType;

    /// <summary>
    /// Creates a <see cref="ServiceDescriptor"/> for service registration.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="key">An optional key for keyed services.</param>
    /// <param name="implementationType">The concrete implementation type.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    /// <returns>A configured <see cref="ServiceDescriptor"/>.</returns>
    private static ServiceDescriptor Generate(Type serviceType,
        string? key,
        Type implementationType,
        ServiceLifetime lifetime)
        => new(serviceType, key, implementationType, lifetime);
}
