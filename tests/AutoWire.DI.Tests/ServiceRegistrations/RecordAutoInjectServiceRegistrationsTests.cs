using System.Reflection;
using AutoWire.DI.Attributes;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AutoWire.DI.Tests.ServiceRegistrations;

public class RecordAutoInjectServiceRegistrationsTests
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
            sd => sd.ImplementationType == typeof(RecordTestService) && sd.Lifetime == ServiceLifetime.Singleton);

        // Check if ScopedService is registered as Scoped
        Assert.Contains(registeredServices,
            sd => sd.ImplementationType == typeof(RecordScopedService) && sd.Lifetime == ServiceLifetime.Scoped);
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
        var type = typeof(RecordNoInjectService);
        registrations.RegisterFromAssembly(assemblyWithNoAttributes);

        // Assert
        Assert.DoesNotContain(registeredServices,
            sd => sd.ImplementationType == type); // We expect no services to be registered as type NoInjectService.
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
        var type = typeof(RecordAbstractService);
        registrations.RegisterFromAssembly(assemblyWithAbstractClass);

        // Assert
        Assert.DoesNotContain(registeredServices,
            sd => sd.ImplementationType == type); // We expect no services to be registered as type AbstractService.
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
            sd => sd.ImplementationType == typeof(RecordBaseService));
        // Only one service should be registered from the derived class
        Assert.Single(registeredServices,
            sd => sd.ImplementationType == typeof(RecordDerivedService) && sd.Lifetime == ServiceLifetime.Singleton);
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
            sd => sd.ImplementationType == typeof(RecordAnotherBaseService));
        // Only one service should be registered from the derived class
        Assert.DoesNotContain(registeredServices,
            sd => sd.ImplementationType == typeof(RecordAnotherDerivedService));
    }

    // Dummy interface for the sake of testing
    private interface IRecordTestService;

    // Example services for testing
    [AutoInject(lifetime: ServiceLifetime.Singleton)]
    private record RecordTestService : IRecordTestService;

    [AutoInject(lifetime: ServiceLifetime.Scoped)]
    private record RecordScopedService : IRecordTestService;

    // Example of a class without AutoInject attribute for negative test
    private record RecordNoInjectService;

    [AutoInject]
    private abstract record RecordAbstractService;

    private abstract record RecordBaseService;

    // Derived class with AutoInjectAttribute
    [AutoInject(lifetime: ServiceLifetime.Singleton)]
    private record RecordDerivedService : RecordBaseService;

    [AutoInject(lifetime: ServiceLifetime.Singleton)]
    private abstract record RecordAnotherBaseService;

    // Derived class with AutoInjectAttribute
    private record RecordAnotherDerivedService : RecordAnotherBaseService;
}
