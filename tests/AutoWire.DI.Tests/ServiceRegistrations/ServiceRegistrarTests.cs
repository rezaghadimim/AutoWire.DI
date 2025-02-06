using AutoWire.DI.Attributes;
using AutoWire.DI.Exceptions;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.ServiceRegistrations;

public class ServiceRegistrarTests
{
    [Fact]
    public void RegisterService_ShouldRegisterServicesWithAutoInjectAttribute()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        var serviceType = typeof(MyService);

        // Act
        serviceRegistrar.RegisterService(serviceType); // Directly register just the target class

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMyService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldThrowException_WhenDuplicateServiceRegistrationOccurs()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        // Two classes both using AutoInjectAttribute with the same key
        var serviceType1 = typeof(MyService);
        var serviceType2 = typeof(AnotherService);

        serviceRegistrar.RegisterService(serviceType1);

        // Act & Assert
        Assert.Throws<DuplicateServiceRegistrationException>(() =>
            serviceRegistrar.RegisterService(serviceType2));
    }

    [Fact]
    public void RegisterService_ShouldDetermineServiceTypeCorrectly_WhenServiceImplementingMultipleInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();
        var registrations = new ServiceRegistrar(services);

        // Assume this class implements multiple interfaces
        var serviceType = typeof(ServiceImplementingMultipleInterfaces);

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
        var registrations = new ServiceRegistrar(services);

        // Assume this class doesn't directly implement an interface and has no ServiceType set
        var serviceType = typeof(ConcreteServiceWithoutInterface); // Class that inherits another class

        // Act
        registrations.RegisterService(serviceType);

        // Assert
        // Check if the service was registered with the class itself, not the base class
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ConcreteServiceWithoutInterface));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldUseMostDerivedInterface_WhenNoDirectInterfaceAndServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        // This class does not directly implement an interface but inherits from a class that does.
        var serviceType = typeof(InheritedServiceWithoutDirectInterface);

        // Act
        serviceRegistrar.RegisterService(serviceType);

        // Assert
        // Ensure the service is registered with the most derived interface (the lowest in the hierarchy).
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMyService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldThrowAmbiguousServiceTypeException_WhenMultipleInterfacesImplementedAndServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        // Class that implements multiple interfaces but does not specify ServiceType
        var serviceType = typeof(ConflictingServiceWithMultipleInterfaces);

        // Act & Assert
        var exception = Assert.Throws<AmbiguousServiceTypeException>(() =>
            serviceRegistrar.RegisterService(serviceType));

        // Ensure the exception message contains the class name
        Assert.Contains(serviceType.FullName!, exception.Message);
    }

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
    private class ServiceImplementingMultipleInterfaces : IMyService, IOtherService
    {
        public void DoSomething() { }
    }

    private interface IOtherService;

    private class MyServiceWithInterface : IMyService
    {
        public void DoSomething() { }
    }

    [AutoInject]
    private class InheritedServiceWithoutDirectInterface : MyServiceWithInterface;

    private class MyBaseServiceWithoutInterface;

    [AutoInject]
    private class ConcreteServiceWithoutInterface : MyBaseServiceWithoutInterface;

    [AutoInject]
    private class ConflictingServiceWithMultipleInterfaces : IMyService, IOtherService
    {
        public void DoSomething() { }
    }
}
