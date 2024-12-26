using System.Reflection;
using AutoWire.DI.Attributes;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> to facilitate automatic service registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers services in the provided <see cref="IServiceCollection"/> that are decorated with the <see cref="AutoInjectAttribute"/>.
    /// This method scans the calling assembly for classes marked with <see cref="AutoInjectAttribute"/> and registers them 
    /// as services in the specified service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> containing the registered services.</returns>
    /// <remarks>
    /// This method uses <see cref="Assembly.GetCallingAssembly"/> to determine the assembly from which the registration 
    /// method was called. It is important to ensure that services marked with <see cref="AutoInjectAttribute"/> are 
    /// defined in that calling assembly; otherwise, no services will be registered.
    /// </remarks>
    public static IServiceCollection RegisterAutoInjectableServices(this IServiceCollection services)
    {
        var registrationServices = new AutoInjectServiceRegistrations(services);
        registrationServices.RegisterFromAssembly(Assembly.GetCallingAssembly());
        return services;
    }
}