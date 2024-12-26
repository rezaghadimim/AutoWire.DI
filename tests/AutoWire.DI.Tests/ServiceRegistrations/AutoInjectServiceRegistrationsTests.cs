using System.Reflection;
using AutoWire.DI.Attributes;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AutoWire.DI.Tests.ServiceRegistrations;

public class AutoInjectServiceRegistrationsTests
{
    [Fact]
    public void RegisterFromAssembly_Should_Register_Types_With_AutoInjectAttribute()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var registeredServices = new List<ServiceDescriptor>();

        serviceCollectionMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
            .Callback<ServiceDescriptor>(d => registeredServices.Add(d));

        var assembly = Assembly.GetExecutingAssembly(); // Use the current assembly which contains test services
        var registrations = new AutoInjectServiceRegistrations(serviceCollectionMock.Object);

        // Act
        registrations.RegisterFromAssembly(assembly);

        // Assert
        Assert.True(registeredServices.Count > 2); // We expect at least two services to be registered (TestService and ScopedService).

        // Check if TestService is registered as Singleton
        Assert.Contains(registeredServices,
            sd => sd.ServiceType == typeof(ITestService) && sd.Lifetime == ServiceLifetime.Singleton);

        // Check if ScopedService is registered as Scoped
        Assert.Contains(registeredServices,
            sd => sd.ServiceType == typeof(ITestService) && sd.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void RegisterFromAssembly_Should_Not_Register_Any_Types_Without_AutoInjectAttribute()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var registeredServices = new List<ServiceDescriptor>();

        serviceCollectionMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
            .Callback<ServiceDescriptor>(d => registeredServices.Add(d));

        // Create a dummy class without the AutoInject attribute
        var assemblyWithNoAttributes = Assembly.GetExecutingAssembly(); // Use the current assembly 

        var registrations = new AutoInjectServiceRegistrations(serviceCollectionMock.Object);

        // Ensure there are no services found without the AutoInject attribute
        var type = typeof(NoInjectService);
        registrations.RegisterFromAssembly(assemblyWithNoAttributes);

        // Assert
        Assert.DoesNotContain(registeredServices,
            sd => sd.ServiceType == type); // We expect no services to be registered as type NoInjectService.
    }

    // Dummy interface for the sake of testing
    private interface ITestService;

    // Example services for testing
    [AutoInject(lifetime: ServiceLifetime.Singleton)]
    private class TestService : ITestService;

    [AutoInject(lifetime: ServiceLifetime.Scoped)]
    private class ScopedService : ITestService;

    // Example of a class without AutoInject attribute for negative test
    private class NoInjectService;
}