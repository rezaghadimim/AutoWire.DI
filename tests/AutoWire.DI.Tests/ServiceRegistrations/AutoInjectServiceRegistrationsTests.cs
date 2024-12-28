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

    [Fact]
    public void RegisterFromAssembly_Should_Not_Register_Any_Abstract_class()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var registeredServices = new List<ServiceDescriptor>();

        serviceCollectionMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
            .Callback<ServiceDescriptor>(d => registeredServices.Add(d));

        // Create a dummy abstract class
        var assemblyWithAbstractClass = Assembly.GetExecutingAssembly(); // Use the current assembly

        var registrations = new AutoInjectServiceRegistrations(serviceCollectionMock.Object);

        // Ensure there are no services found without the AutoInject attribute
        var type = typeof(AbstractService);
        registrations.RegisterFromAssembly(assemblyWithAbstractClass);

        // Assert
        Assert.DoesNotContain(registeredServices,
            sd => sd.ServiceType == type); // We expect no services to be registered as type AbstractService.
    }

    [Fact]
    public void RegisterFromAssembly_Should_Register_Derived_Classes_With_AutoInjectAttribute()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var registeredServices = new List<ServiceDescriptor>();

        // Mock the "Add" method to capture service registrations
        serviceCollectionMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
            .Callback<ServiceDescriptor>(d => registeredServices.Add(d));

        var assembly = Assembly.GetExecutingAssembly(); // Scanning the current assembly
        var registration = new AutoInjectServiceRegistrations(serviceCollectionMock.Object);

        // Act
        registration.RegisterFromAssembly(assembly);

        // Assert
        // Check if DerivedService has been registered
        Assert.DoesNotContain(registeredServices,
            sd => sd.ServiceType == typeof(BaseService));
        // Only one service should be registered from the derived class
        Assert.Single(registeredServices,
            sd => sd.ServiceType == typeof(DerivedService) && sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void RegisterFromAssembly_Should_Not_Register_Derived_Classes_With_AutoInjectAttribute_From_Abstract_Parent()
    {
        // Arrange
        var serviceCollectionMock = new Mock<IServiceCollection>();
        var registeredServices = new List<ServiceDescriptor>();

        // Mock the "Add" method to capture service registrations
        serviceCollectionMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>()))
            .Callback<ServiceDescriptor>(d => registeredServices.Add(d));

        var assembly = Assembly.GetExecutingAssembly(); // Scanning the current assembly
        var registration = new AutoInjectServiceRegistrations(serviceCollectionMock.Object);

        // Act
        registration.RegisterFromAssembly(assembly);

        // Assert
        // Check if DerivedService has been registered
        Assert.DoesNotContain(registeredServices,
            sd => sd.ServiceType == typeof(AnotherBaseService));
        // Only one service should be registered from the derived class
        Assert.DoesNotContain(registeredServices,
            sd => sd.ServiceType == typeof(AnotherDerivedService));
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

    [AutoInject]
    private abstract class AbstractService;

    private abstract class BaseService;

    // Derived class with AutoInjectAttribute
    [AutoInject(lifetime: ServiceLifetime.Singleton)]
    private class DerivedService : BaseService;

    [AutoInject(lifetime: ServiceLifetime.Singleton)]
    private abstract class AnotherBaseService;

    // Derived class with AutoInjectAttribute
    private class AnotherDerivedService : AnotherBaseService;
}
