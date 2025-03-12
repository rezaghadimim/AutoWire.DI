using AutoWire.DI.Attributes;
using AutoWire.DI.ServiceRegistrations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.ServiceRegistrations;

public class KeyedServiceRegistrarTests
{
    [Fact]
    public void RegisterService_ShouldRegisterServiceWithKeyedAutoInjectAttributeAndEqualServiceName()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceRegistrar = new ServiceRegistrar(services);

        var serviceType = typeof(KeyedService);

        // Act
        serviceRegistrar.RegisterService(serviceType);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IMyService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(serviceType, serviceDescriptor.KeyedImplementationType);
        Assert.Equal("KeyedService", serviceDescriptor.ServiceKey);
    }

    private interface IBaseService;

    private interface IMyService : IBaseService
    {
        void DoSomething();
    }

    [KeyedAutoInject]
    private class KeyedService : IMyService
    {
        public void DoSomething() { }
    }
}
