using AutoWire.DI.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Attributes;

/// <summary>
/// Indicates that a class should be automatically injected by the dependency injection container.
/// </summary>
/// <remarks>
/// This attribute can be used on classes that require dependency injection to specify that they should
/// be resolved and managed by the dependency injection system. It supports specifying a key and
/// the lifetime of the service to control how instances of the class are created and managed.
///
/// Note that this attribute does not support inheritance, meaning that derived classes will
/// not inherit this attribute from their base classes.
/// </remarks>
/// <example>
/// <code>
/// [AutoInject("myServiceKey", ServiceLifetime.Singleton)]
/// public class MyService
/// {
///     // Implementation details
/// }
/// </code>
/// <code>
/// [AutoInject]
/// public class DefaultScopedService
/// {
///     // Implementation details
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoInjectAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoInjectAttribute"/> class with an optional key, service lifetime, and service type.
    /// </summary>
    /// <param name="key">An optional key to identify the service instance. If not specified, the service is registered without a key.</param>
    /// <param name="lifetime">The lifetime of the service to be registered. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
    /// <param name="serviceType">
    /// The optional service type to be registered. If not specified, one of the implemented interfaces will be used automatically.
    /// If no interfaces are implemented, the class type itself will be used.
    /// <strong>Note:</strong> It is strongly recommended to explicitly specify the service type in scenarios where multiple interfaces are implemented by the class to avoid ambiguity.
    /// <para>
    /// If the class implements multiple interfaces and <paramref name="serviceType"/> is not provided,
    /// an <see cref="AutoInjectRegistrationException"/> will be thrown to avoid ambiguity.
    /// </para>
    /// </param>
    /// <exception cref="AutoInjectRegistrationException">
    /// Thrown when multiple interfaces are implemented but <paramref name="serviceType"/> is not specified.
    /// </exception>
    public AutoInjectAttribute(string? key = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Type? serviceType = null)
    {
        Lifetime = lifetime;
        Key = key;
        ServiceType = serviceType;
    }

    /// <summary>
    /// Gets the service lifetime of the attribute.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Gets the optional key associated with the service.
    /// </summary>
    public string? Key { get; }

    /// <summary>
    /// Gets the service type to be registered.
    /// </summary>
    /// <remarks>
    /// If this property is null, the system will attempt to use one of the implemented interfaces of the class.
    /// If the class does not implement any interfaces, the class type itself will be used as the service type.
    /// </remarks>
    public Type? ServiceType { get; }
}
