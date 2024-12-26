using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoInjectAttribute(string? key = null, ServiceLifetime lifetime = ServiceLifetime.Scoped) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;

    public string? Key { get; } = key;
}