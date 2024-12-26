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
    /// Initializes a new instance of the <see cref="AutoInjectAttribute"/> class with an optional key and service lifetime.
    /// </summary>
    /// <param name="key">An optional key to identify the service instance.</param>
    /// <param name="lifetime">The lifetime of the service to be registered. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
    public AutoInjectAttribute(string? key = null,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        Lifetime = lifetime;
        Key = key;
    }

    /// <summary>
    /// Gets the service lifetime of the attribute.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Gets the optional key associated with the service.
    /// </summary>
    public string? Key { get; }
}