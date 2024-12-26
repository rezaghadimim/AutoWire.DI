using AutoWire.DI.Attributes;
using AutoWire.DI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AutoWire.DI.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void RegisterAutoInjectableServices_ShouldRegisterAutoInjectServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        
        // Act
        serviceCollection.RegisterAutoInjectableServices();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        var myService = serviceProvider.GetService<IMyService>();
        Assert.NotNull(myService);
        Assert.IsType<MyService>(myService);
    }
}

// Sample service interface
public interface IMyService;

// Sample service implementation
[AutoInject]
public class MyService : IMyService;
