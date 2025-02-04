using AutoWire.DI.Attributes;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.ServiceRegistrations;

public class GenericServiceRegistrarTests
{
    [Fact]
    public void RegisterService_ShouldRegisterWithInterface_WhenServiceTypeIsNotNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        // Class that implements a generic interface
        var serviceType = typeof(GenericService<>);

        // Act
        serviceRegistrar.RegisterService(serviceType);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IGenericService<>));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldRegisterWithInterface_WhenServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        // Class that implements a generic interface
        var serviceType = typeof(AnotherGenericService<>);

        // Act
        serviceRegistrar.RegisterService(serviceType);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IGenericService<>));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void RegisterService_ShouldRegisterWithoutInterface_WhenServiceTypeIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        // Class that is generic but does not implement an interface
        var serviceType = typeof(MyGenericServiceWithoutInterface<>);

        // Act
        serviceRegistrar.RegisterService(serviceType);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(MyGenericServiceWithoutInterface<>));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
    }

    private interface IGenericService<T>;

    [AutoInject(serviceType: typeof(IGenericService<>))]
    private class GenericService<T> : IGenericService<T>;

    [AutoInject]
    private class AnotherGenericService<T> : IGenericService<T>;

    [AutoInject]
    private class MyGenericServiceWithoutInterface<T>;
}
