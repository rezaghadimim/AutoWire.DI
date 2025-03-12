using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Attributes;

/// <summary>
/// An attribute that marks a class for automatic registration in the dependency injection container with a specific key.
/// </summary>
/// <inheritdoc />
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class KeyedAutoInjectAttribute : AutoInjectAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyedAutoInjectAttribute"/> class.
    /// </summary>
    /// <param name="lifetime">The service lifetime, indicating how the service is managed.</param>
    /// <param name="serviceType">The specific service type to register. If null, defaults to the type of the class this attribute is applied to.</param>
    public KeyedAutoInjectAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped,
        Type? serviceType = null) : base(null, lifetime, serviceType)
    {
    }
}
