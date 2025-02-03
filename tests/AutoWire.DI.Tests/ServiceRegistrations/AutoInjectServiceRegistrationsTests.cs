using AutoWire.DI.Attributes;
using AutoWire.DI.Exceptions;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.ServiceRegistrations;

public class AutoInjectServiceRegistrationsTests
{
    [Fact]
    public void RegisterFromAssembly_ShouldRegisterServicesWithAutoInjectAttribute()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new AutoInjectServiceRegistrations(services);

        var serviceType = typeof(MyService);

        // Act
        registrations.RegisterService(serviceType); // Directly register just the target class

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMyService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterFromAssembly_ShouldThrowException_WhenDuplicateServiceRegistrationOccurs()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new AutoInjectServiceRegistrations(services);

        // Two classes both using AutoInjectAttribute with the same key
        var serviceType1 = typeof(MyService);
        var serviceType2 = typeof(AnotherService);

        registrations.RegisterService(serviceType1);

        // Act & Assert
        Assert.Throws<DuplicateServiceRegistrationException>(() =>
            registrations.RegisterService(serviceType2));
    }

    [Fact]
    public void RegisterService_ShouldDetermineServiceTypeCorrectly_WhenMultipleInterfacesImplemented()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new AutoInjectServiceRegistrations(services);

        // Assume this class implements multiple interfaces
        var serviceType = typeof(MultiInterfaceService);

        // Act
        registrations.RegisterService(serviceType);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMyService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldUseClassItself_WhenNoInterfaceAndServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new AutoInjectServiceRegistrations(services);

        // Assume this class doesn't directly implement an interface and has no ServiceType set
        var serviceType = typeof(MyServiceWithoutInterface); // Class that inherits another class

        // Act
        registrations.RegisterService(serviceType);

        // Assert
        // Check if the service was registered with the class itself, not the base class
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(MyServiceWithoutInterface));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldUseBaseInterface_WhenNoDirectInterfaceAndServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new AutoInjectServiceRegistrations(services);

        // Assume this class doesn't directly implement an interface and has no ServiceType set
        var serviceType = typeof(MyServiceWithoutDirectInterface); // Class that inherits another class

        // Act
        registrations.RegisterService(serviceType);

        // Assert
        // Check if the service was registered with the class itself, not the base class
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMyService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldThrowAmbiguousServiceTypeException_WhenMultipleInterfacesImplementedAndServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new AutoInjectServiceRegistrations(services);

        // Class that implements multiple interfaces but does not specify ServiceType
        var serviceType = typeof(AmbiguousService);

        // Act & Assert
        var exception = Assert.Throws<AmbiguousServiceTypeException>(() =>
            registrations.RegisterService(serviceType));

        // Ensure the exception message contains the class name
        Assert.Contains(serviceType.FullName, exception.Message);
    }

    // Sample test classes
    [AutoInject]
    private class MyService : IMyService
    {
        public void DoSomething() { }
    }

    private interface IBaseService;

    private interface IMyService : IBaseService
    {
        void DoSomething();
    }

    [AutoInject]
    private class AnotherService : IMyService
    {
        public void DoSomething() { }
    }

    [AutoInject(serviceType: typeof(IMyService))]
    private class MultiInterfaceService : IMyService, IOtherService
    {
        public void DoSomething() { }
    }

    private interface IOtherService;

    private class MyServiceWithInterface : IMyService
    {
        public void DoSomething() { }
    }

    [AutoInject]
    private class MyServiceWithoutDirectInterface : MyServiceWithInterface;

    private class MyBaseServiceWithoutInterface;

    [AutoInject]
    private class MyServiceWithoutInterface : MyBaseServiceWithoutInterface;

    [AutoInject]
    private class AmbiguousService : IMyService, IOtherService
    {
        public void DoSomething() { }
    }
}
